using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TBTv3Key.Core;

namespace TBTv3Key
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtCalibGenKey_Click(object sender, RoutedEventArgs e)
        {
            string seed = TxtCalibCode.Text.Trim();
            string key = MyCopyright.ComputeSha256($"lock.calib.{seed}").Substring(0, 8);
            TxtCalibKey.Text = key;
        }
    }
}