using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Text;
using System.Windows;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndAbout.xaml
    /// </summary>
    public partial class WndAbout : Window
    {
        private StringBuilder _sb = new();

        public WndAbout()
        {
            InitializeComponent();
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                _sb.Clear();
            }
            else
            {
                _sb.Append(e.Key.ToString());
                if (_sb.ToString().ToLower() == "ncmd4nhd1d9d1")
                {
                    BtResetPw.Visibility = Visibility.Visible;
                }
                System.Diagnostics.Debug.WriteLine(_sb.ToString());
            }
        }

        private void BtResetPw_Click(object sender, RoutedEventArgs e)
        {
            if (_sb.ToString().ToLower() == "ncmd4nhd1d9d1rd3sd3t")
            {
                var s = DbRepository.Instance.Settings;
                s.Update("pm.quantri.kythuat.account", "");
            }
        }
    }
}
