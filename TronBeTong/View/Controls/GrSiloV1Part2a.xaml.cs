using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for SiloV1Part2a.xaml
    /// </summary>
    public partial class GrSiloV1Part2a : UserControl
    {
        #region ZText
        public string ZText
        {
            get { return (string)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }
        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(GrSiloV1Part2a), new PropertyMetadata(null));
        #endregion

        #region ZForeground
        public Brush ZForeground
        {
            get { return (Brush)GetValue(ZForegroundProperty); }
            set { SetValue(ZForegroundProperty, value); }
        }
        public static readonly DependencyProperty ZForegroundProperty =
            DependencyProperty.Register("ZForeground", typeof(Brush), typeof(GrSiloV1Part2a), new PropertyMetadata(Brushes.Black, OnZForegroundPropertyChanged));

        private static void OnZForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GrSiloV1Part2a s)
            {
                s.TxtInput.Foreground = s.ZForeground;
            }
        }
        #endregion
        public bool IsReadonly
        {
            get { return TxtInput.IsReadOnly; }
            set
            {
                TxtInput.IsReadOnly = value;
                TxtInput.Background = IsReadonly ? Brushes.WhiteSmoke : Brushes.White;
            }
        }

        public GrSiloV1Part2a()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void SetBrush(Brush b)
        {
            ImgSep.ZBlend = b;
            ImgBody.ZBlend = b;
        }
    }
}
