using System;
using System.Windows;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for ChangePassWnd.xaml
    /// </summary>
    public partial class WndChangePass : Window
    {
        public WndChangePass()
        {
            InitializeComponent();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            var s = DbRepository.Instance.Settings;
            var oldpass = s.GetValue("sim.pw");

            if ((string.IsNullOrEmpty(oldpass) || oldpass == PwOld.Password) && !string.IsNullOrEmpty(PwNew.Password) && PwNew.Password == PwRetype.Password)
            {
                s.Update("sim.pw", PwNew.Password);
                DialogResult = true;
            }
        }
    }
}
