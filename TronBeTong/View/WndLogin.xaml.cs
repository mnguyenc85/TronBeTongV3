using System;
using System.Windows;
using System.Windows.Input;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for LoginWnd.xaml
    /// </summary>
    public partial class WndLogin : Window
    {
        public PMNhanVienDO? Account { get; set; }

        public WndLogin()
        {
            InitializeComponent();
        }
        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void PnlTitle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtTest_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblErrPW.Visibility = Visibility.Collapsed;
        }

        private void TxtUser_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LblErrUser.Visibility = Visibility.Collapsed;
        }

        private void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUser.Focus();
        }

        private void TxtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Login()
        {
            if (string.IsNullOrEmpty(TxtUser.Text))
            {
                LblErrUser.Visibility = Visibility.Visible;
                return;
            }
            if (string.IsNullOrEmpty(TxtPW.Password))
            {
                LblErrPW.Visibility = Visibility.Visible;
                return;
            }
            Account = new() { UserName = TxtUser.Text, Password = TxtPW.Password };
            DialogResult = true;
        }
    }
}
