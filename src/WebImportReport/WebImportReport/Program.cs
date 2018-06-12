using System;
using System.IO;
using System.Linq;
using WebImportReport.Common;

namespace WebImportReport
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Count() < 1)
                {
                    Console.WriteLine("WebImportReport <input dir>");
                    Console.WriteLine("\n\rPress enter to exit");
                    Console.ReadLine();
                    return;
                }

                var inputDir = args[0];
                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine($"Input dir {inputDir} does not exist");
                    Console.WriteLine("\n\rPress enter to exit");
                    Console.ReadLine();
                    return;
                }

                var converter = new ConvertToCSV();
                var statuses = converter.ConvertFiles(inputDir, "paamelding*.xml");
                if (statuses.Any())
                {
                    var outputStatuses = string.Join("\n\r", statuses);
                    Console.WriteLine("\n\r");
                    Console.WriteLine(outputStatuses);

                }
                Console.WriteLine("\n\rPress enter to exit");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error occured.");
                Console.WriteLine(exception.Message);
                Console.WriteLine("\n\rPress enter to exit");
                Console.ReadLine();
            }
        }
    }
}
