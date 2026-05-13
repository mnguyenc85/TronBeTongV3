using System.Windows;
using DebuggerViewer.ViewModel;

namespace DebuggerViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM _vm = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.Init(plotView);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #region Menu
        private void MniLoadClipboard_Click(object sender, RoutedEventArgs e)
        {
            _vm.LoadDataFromClipBoard();
            _vm.ShowPlot();
        }
        #endregion
    }
}