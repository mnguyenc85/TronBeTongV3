using System;
using System.Windows.Controls;
using TronBeTongV3.Comm;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlTPBangTai.xaml
    /// </summary>
    public partial class CtrlTPBangTai : UserControl
    {
        public ModelHeThong? TramTron { get; set; }

        public CtrlTPBangTai()
        {
            InitializeComponent();
        }

        public void UpdateView(double delta)
        {
            if (TramTron != null)
            {
                if (TramTron.BangTaiNgang.IsChanged)
                {
                    BTNgang.ZState = (int)TramTron.BangTaiNgang.Value;
                }
                if (TramTron.BangTaiXien.IsChanged)
                {
                    BTXien.ZState = (int)TramTron.BangTaiXien.Value;
                }
            }
        }
    }
}
