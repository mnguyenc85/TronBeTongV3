using Serilog;
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
    /// Interaction logic for RegisterWnd.xaml
    /// </summary>
    public partial class WndRegister : Window
    {
        public WndRegister()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string? s = "TBT" + MyCopyright.GetPCFootPrint();
                if (s != null)
                    TxtMaPC.Text = MyCopyright.ComputeMD5(s);
            }
            catch (Exception ex)
            {
                Log.Error($"Register: {ex.Message}");
            }
        }
    }
}
