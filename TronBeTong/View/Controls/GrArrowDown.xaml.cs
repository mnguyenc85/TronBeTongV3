using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for ArrowDown.xaml
    /// </summary>
    public partial class GrArrowDown : UserControl
    {
        private double _t0;
        //private double _t;
        private int _state = 3;

        public GrArrowDown()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (LblTime.Visibility == Visibility.Visible)
            {
                double t = DateTime.Now.Ticks / 10000000d;
                LblTime.Text = (t - _t0).ToString("F1");
            }
        }

        public void SetState(int s)
        {
            if (s != _state)
            {
                ImgArr1.Visibility = s == 1 ? Visibility.Visible : Visibility.Hidden;
                ImgArr2.Visibility = s == 2 ? Visibility.Visible : Visibility.Hidden;

                LblTime.Visibility = s > 0? Visibility.Visible : Visibility.Hidden;
                if (s > 0) _t0 = DateTime.Now.Ticks / 10000000d;

                _state = s;
            }
        }
    }
}
