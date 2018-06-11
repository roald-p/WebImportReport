using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonConvertCSV
{
    public class MainViewModel : ViewModelBase
    {
        private string m_inputDir;

        public string InputDir
        {
            get => m_inputDir;
            set
            {
                m_inputDir = value;
                OnPropertyChanged(nameof(InputDir));
            }
        }
    }
}
