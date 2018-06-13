using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Prism.Commands;
using WebImportReport.Common;

namespace WebImportReportUI
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Folder_Path))
            {
                InputDir = Properties.Settings.Default.Folder_Path;
            }
        }

        private DelegateCommand m_selectPathCommand;
        private DelegateCommand m_generateCSVCommand;
        private DelegateCommand m_openFolderCommand;

        private string m_inputDir;
        private string m_filePattern= "paamelding*.xml";
        private string m_matchingFiles;
        private string m_convertResult;
        private bool m_directoryExists;

        public string InputDir
        {
            get => m_inputDir;
            set
            {
                m_inputDir = value;
                OnPropertyChanged(nameof(InputDir));
                OnPropertyChanged(nameof(InputDirectoryExists));
                MatchingFiles = FoundFiles();
            }
        }

        public string FilePattern
        {
            get => m_filePattern;
            set
            {
                m_filePattern = value;
                OnPropertyChanged(nameof(FilePattern));
                MatchingFiles = FoundFiles();
            }
        }

        public string MatchingFiles
        {
            get => m_matchingFiles;
            set
            {
                m_matchingFiles = value;
                OnPropertyChanged(nameof(MatchingFiles));
            }
        }

        public string ConvertResult
        {
            get => m_convertResult;
            set
            {
                m_convertResult = value;
                OnPropertyChanged(nameof(ConvertResult));
            }
        }

        public ICommand SelectPathCommand
        {
            get
            {
                if (m_selectPathCommand == null)
                {
                    m_selectPathCommand = new DelegateCommand(this.SelectPathCommandExecute);
                }

                return m_selectPathCommand;
            }
        }

        public ICommand GenerateCSVCommand
        {
            get
            {
                if (m_generateCSVCommand == null)
                {
                    m_generateCSVCommand = new DelegateCommand(this.GenerateCSVCommandExecute);
                }

                return m_generateCSVCommand;
            }
        }

        public ICommand OpenFolderCommand
        {
            get
            {
                if (m_openFolderCommand == null)
                {
                    m_openFolderCommand = new DelegateCommand(this.OpenFolderCommandExecute);
                }

                return m_openFolderCommand;
            }
        }
        
        public void SelectPathCommandExecute()
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = Properties.Settings.Default.Folder_Path;

                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    InputDir = dlg.SelectedPath;
                    Properties.Settings.Default.Folder_Path = dlg.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public void OpenFolderCommandExecute()
        {
            Process.Start(m_inputDir);
        }

        public bool InputDirectoryExists
        {
            get
            {
                return Directory.Exists(m_inputDir);
            }
        }

        public void GenerateCSVCommandExecute()
        {
            if (string.IsNullOrWhiteSpace(m_inputDir))
            {
                ConvertResult = "Må velge input katalog først";
                return;
            }

            if (!Directory.Exists(m_inputDir))
            {
                ConvertResult = $"Katalogen finnes ikke {m_inputDir}";
                return;
            }

            if (string.IsNullOrWhiteSpace(m_filePattern))
            {
                ConvertResult = "Må angi filmønster, f.eks. \"paamelding *.xml\"";
                return;
            }

            var converter = new ConvertToCSV();
            var statuses = converter.ConvertFiles(m_inputDir, m_filePattern);
            if (!statuses.Any())
            {
                ConvertResult = "Ingen filer konvertert";
                return;
            }

            var results = string.Join("\n\r", statuses);
            ConvertResult = $"Resultat konvertering:\n\r{results}";
        }

        private string FoundFiles()
        {
            if (!Directory.Exists(m_inputDir))
            {
                return $"Katalog finnes ikke {m_inputDir}";
            }
            var inputfiles = Directory.EnumerateFiles(m_inputDir, m_filePattern);
            if (!inputfiles.Any())
            {
                return $"Ingen filer funnet på katalogen {m_inputDir} med mønster {m_filePattern}";
            }

            return string.Join("\n\r", inputfiles);
        }
    }
}
