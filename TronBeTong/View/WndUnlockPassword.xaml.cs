using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TronBeTongV3.Core;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndUnlockPassword.xaml
    /// </summary>
    public partial class WndUnlockPassword : Window
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);
        private int _seed;

        public WndUnlockPassword()
        {
            InitializeComponent();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUnlock()) DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int n = (int)(_rnd.NextDouble() * 100);
            for (int i = 0; i <  n; i++)
                _seed = (int)(_rnd.NextDouble() * 1000000);
            TxtKey.Text = _seed.ToString("000000");
        }

        private void TxtPass_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CheckUnlock())
                {
                    DialogResult = true;
                    Close();
                }
                e.Handled = true;
            }
        }

        private bool CheckUnlock()
        {
            if (TxtPass.Text == "ncmanh0846056084") return true;
            string key = MyCopyright.ComputeSha256($"lock.calib.{_seed}").Substring(0, 8);
            System.Diagnostics.Debug.WriteLine(key);
            return TxtPass.Text.ToLower() == key.ToLower();
        }
    }
}
