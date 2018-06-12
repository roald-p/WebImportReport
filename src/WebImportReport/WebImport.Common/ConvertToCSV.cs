using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace WebImportReport.Common
{
    public class ConvertToCSV
    {
        public List<string> ConvertFiles(string inputDir, string filePattern)
        {
            var statuses = new List<string>();
            var skstream = GetEmbeddedResourceStream("WebImportReport.Common.skytterlag-og-od.csv");
            if (skstream == null)
            {
                statuses.Add("Could not load embedded resource \"WebImportReport.Common.skytterlag-og-od.csv\"");
                return statuses;
            }

            var myreader = new StreamReader(skstream, Encoding.GetEncoding("ISO-8859-1"));
            string line;
            var skytterlagList = new Dictionary<string, SkytterlagMetaData>();

            while ((line = myreader.ReadLine()) != null)
            {

                var touples = line.Split(';');
                if (!skytterlagList.ContainsKey(touples[0]))
                {
                    var newTeam = new SkytterlagMetaData();
                    newTeam.SkytterlagId = touples[0];
                    newTeam.SkytterlagNavn = touples[1];
                    newTeam.CommunityNumber = touples[2];
                    newTeam.CommunityName = touples[3];
                    newTeam.ParentId = touples[4];
                    newTeam.ParentName = touples[5];
                    newTeam.Category = touples[6];

                    skytterlagList.Add(touples[0], newTeam);
                }
            }

            var inputfiles = Directory.EnumerateFiles(inputDir, filePattern);
            foreach (var inputfile in inputfiles)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(paamelding));

                StreamReader reader = new StreamReader(inputfile, Encoding.GetEncoding("iso-8859-1"));
                var leonImport = (paamelding)serializer.Deserialize(reader);
                reader.Close();

                List<string> skyttere = new List<string>();
                skyttere.Add("Skytterlag;Hold;Dag;Dato;Opprop;Skytetid;Lag;Skive;Fornavn;Etternavn;Klasse");
                foreach (var ovelse in leonImport.ovelse)
                {
                    foreach (var skytter in ovelse.paameldingskytter)
                    {
                        SkytterlagMetaData skytterlag;
                        if (skytterlagList.ContainsKey(skytter.sklagnr))
                        {
                            skytterlag = skytterlagList[skytter.sklagnr];
                        }
                        else
                        {
                            skytterlag = new SkytterlagMetaData
                            {
                                SkytterlagId = skytter.sklagnr,
                                SkytterlagNavn = "NA",
                                CommunityNumber = "NA",
                                CommunityName = "NA",
                                ParentId = "NA",
                                ParentName = "NA",
                                Category = "NA"
                            };

                            statuses.Add($"missing mapping for skytterlagnr {skytter.sklagnr}");
                        }

                        var lag = ovelse.lag.First(l => l.lagnr == skytter.lag);
                        var dato = DateTime.Parse(lag.dato);
                        var skytterstr =
                            $"{skytterlag.SkytterlagNavn};{ovelse.hold};{dato.ToString("dddd")};{dato.ToString("dd.MMMM")};{lag.klopprop};{lag.klskytetid};{lag.lagnr};{skytter.skive};{skytter.fornavn};{skytter.etternavn};{skytter.klasse}";
                        skyttere.Add(skytterstr);
                    }
                }

                var path = Path.GetDirectoryName(inputfile);
                var fileWithoutExt = Path.GetFileNameWithoutExtension(inputfile);
                var outputfile = $"{fileWithoutExt}.generated.csv";
                var output = Path.Combine(path, outputfile);
                File.WriteAllLines(output, skyttere, Encoding.GetEncoding("ISO-8859-1"));
                statuses.Add(output);
            }

            return statuses;
        }

        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
        public static string[] GetEmbeddedResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
    }
}
