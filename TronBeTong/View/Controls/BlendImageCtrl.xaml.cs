using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for BlendImageCtrl.xaml
    /// </summary>
    public partial class BlendImageCtrl : UserControl
    {
        public ImageSource ZSource
        {
            get { return (ImageSource)GetValue(ZSourceProperty); }
            set { SetValue(ZSourceProperty, value); }
        }

        public static readonly DependencyProperty ZSourceProperty =
            DependencyProperty.Register("ZSource", typeof(ImageSource), typeof(BlendImageCtrl), new PropertyMetadata(null));

        public Brush? ZBlend
        {
            get { return (Brush)GetValue(ZBlendProperty); }
            set { SetValue(ZBlendProperty, value); }
        }

        public static readonly DependencyProperty ZBlendProperty =
            DependencyProperty.Register("ZBlend", typeof(Brush), typeof(BlendImageCtrl), new PropertyMetadata(null));

        public BlendImageCtrl()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
        }
    }
}
