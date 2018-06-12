using System.Windows;

namespace WebImportReportUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var model = new MainViewModel();
            this.DataContext = model;
            InitializeComponent();
        }
    }
}
