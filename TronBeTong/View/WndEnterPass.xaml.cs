using System;
using System.Windows;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for EnterPassWnd.xaml
    /// </summary>
    public partial class WndEnterPass : Window
    {
        public string? PassType { get; set; }
        public WndEnterPass()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPass.Focus();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            CheckPw();
        }

        private void TxtPass_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CheckPw();
                Close();
            }
        }

        private void CheckPw()
        {
            var s = DbRepository.Instance.Settings;
            if (PassType != null && s.GetValue(PassType) == TxtPass.Password)
            {
                DialogResult = true;
                return;
            }

            DialogResult = false;
        }
    }
}
