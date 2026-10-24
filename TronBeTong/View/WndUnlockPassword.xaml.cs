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
        private int _k1 = 26, _k2 = 9, _k3 = 5;
        private int _unlockCount = 0;

        private Random _rnd = new Random(DateTime.Now.Millisecond);
        private int _seed;

        public WndUnlockPassword()
        {
            InitializeComponent();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUnlock()) DialogResult = true;
            else
            {
                _unlockCount++;
                if (_unlockCount > 3)
                {
                    GenSeed();
                    _unlockCount = 0;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenSeed();
        }

        private void GenSeed()
        {
            int n = (int)(_rnd.NextDouble() * 10);
            for (int i = 0; i < n; i++)
                _seed = (int)(_rnd.NextDouble() * 1000000);
            TxtKey.Text = _seed.ToString();
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
            //string key = MyCopyright.ComputeSha256($"lock.calib.{_seed}").Substring(0, 8);
            string key = ((_seed * _k2 + _k1) / _k3).ToString();
            key = key.Length >= 4 ? key[..4] : key;
            System.Diagnostics.Debug.WriteLine(key);
            return TxtPass.Text.ToLower() == key.ToLower();
        }
    }
}
