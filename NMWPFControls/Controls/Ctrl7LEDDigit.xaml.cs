using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for Ctrl7LEDDigit.xaml
    /// </summary>
    public partial class Ctrl7LEDDigit : UserControl
    {
        #region ZDigitShadow
        public bool ZDigitShadow
        {
            get { return (bool)GetValue(ZDigitShadowProperty); }
            set { SetValue(ZDigitShadowProperty, value); }
        }

        public static readonly DependencyProperty ZDigitShadowProperty =
            DependencyProperty.Register("ZDigitShadow", typeof(bool), typeof(Ctrl7LEDDigit), new PropertyMetadata(true, ZDigitShadowChanged));

        private static void ZDigitShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LEDDigit ctrl)
            {
                ctrl.LblShadow.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region ZShadowBrush
        public Brush? ZShadowBrush
        {
            get { return (Brush?)GetValue(ZShadowBrushProperty); }
            set { SetValue(ZShadowBrushProperty, value); }
        }

        public static readonly DependencyProperty ZShadowBrushProperty =
            DependencyProperty.Register("ZShadowBrush", typeof(Brush), typeof(Ctrl7LEDDigit), new PropertyMetadata(null, ZShadowBrushChanged));

        private static void ZShadowBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LEDDigit c)
            {
                c.LblShadow.Foreground = (Brush?)e.NewValue;
            }
        }
        #endregion

        #region ZText
        public string? ZText
        {
            get { return (string?)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }

        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(Ctrl7LEDDigit), new PropertyMetadata(null, ZTextChanged));

        private static void ZTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LEDDigit ctrl)
            {
                ctrl.LblDigit.Text = (string?)e.NewValue;
            }
        }
        #endregion

        public Ctrl7LEDDigit()
        {
            InitializeComponent();
        }
    }
}
