using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TronBeTongV3.Core;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for SiloV1Part3.xaml
    /// </summary>
    public partial class GrSiloV1Part3 : UserControl
    {
        private bool _zon = true;
        public bool ZOn
        {
            get { return _zon; }
            set { if (_zon != value) { _zon = value; SetZOn(); } }
        }

        public GrSiloV1Part3()
        {
            InitializeComponent();
        }

        public void SetBrush(Brush b)
        {
            ImgRect1.ZBlend = b;
            ImgRect2.ZBlend = b;
        }

        public void SetMode(int m)
        {
            PnlRect2.Visibility = (m & 1) == 1? Visibility.Visible : Visibility.Collapsed;
            ImgConveyor.Visibility = (m & 2) == 2? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetZOn()
        {
            LEDArrow.Visibility = _zon ? Visibility.Visible : Visibility.Hidden;
            ImgConveyor.ZBlend = _zon ? MyBrushes.BrTransparentOn : MyBrushes.BrTransparentOff;
        }
    }
}
