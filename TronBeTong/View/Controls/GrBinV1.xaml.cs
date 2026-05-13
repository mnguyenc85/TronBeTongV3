using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for BinV1.xaml
    /// </summary>
    public partial class GrBinV1 : UserControl
    {
        public GrBinV1()
        {
            InitializeComponent();

            
        }

        public void SetBrush(Brush b)
        {
            ImgRect1.ZBlend = b;
            ImgRect2.ZBlend = b;
            ImgRect3.ZBlend = b;
        }

        public void UpdateView(int on1, int on2, double delta)
        {
            if (on1 >= 0)
            {
                Arrow1.SetState(on1);
                BtOn1.ZState = on1;
            }
            if (on2 >= 0)
            {
                Arrow2.SetState(on2);
                BtOn2.ZState = on2;
            }
        }
    }
}
