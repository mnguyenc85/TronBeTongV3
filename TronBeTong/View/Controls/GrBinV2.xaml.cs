using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for BinV2.xaml
    /// </summary>
    public partial class GrBinV2 : UserControl
    {
        public GrBinV2()
        {
            InitializeComponent();
        }
        
        public void SetBrush(Brush b)
        {
            ImgRect1.ZBlend = b;
            ImgRect2.ZBlend = b;
        }

        public void UpdateView(int on1, double delta)
        {
            if (on1 >= 0)
            {
                Arrow1.SetState(on1);
                BtOn1.ZState = on1;
            }
        }

        public void UpdateArrowView(double delta)
        {
            Arrow1.Update();
        }

        public void SetTimeOn(double t1)
        {
        }
    }
}
