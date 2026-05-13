using ManualPrintDocket.CSDL;
using ManualPrintDocket.ViewModel;
using System.IO;
using System.Windows;

namespace ManualPrintDocket.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWndVM _vm = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var r = DbRepository.Instance;

            // Lấy ds mẫu phiếu
            ListAllTemplates(r.ReportsPath);
        }

        private void ListAllTemplates(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath, "*.frx", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    CboDocketTemplates.Items.Add(Path.GetFileName(file));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Thư mục không tồn tại!");
            }
        }

    }
}