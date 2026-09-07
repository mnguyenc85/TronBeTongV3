using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for SiloV1Part2.xaml
    /// </summary>
    public partial class GrSiloV1Part2 : UserControl
    {
        #region ZText
        private bool _valueChanged = false;
        public string ZText
        {
            get { return (string)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); _valueChanged = false; }
        }
        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(GrSiloV1Part2), new PropertyMetadata(null));
        #endregion

        #region ZForeground
        public Brush ZForeground
        {
            get { return (Brush)GetValue(ZForegroundProperty); }
            set { SetValue(ZForegroundProperty, value); }
        }
        public static readonly DependencyProperty ZForegroundProperty =
            DependencyProperty.Register("ZForeground", typeof(Brush), typeof(GrSiloV1Part2), new PropertyMetadata(Brushes.Black, OnZForegroundPropertyChanged));

        private static void OnZForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GrSiloV1Part2 s)
            {
                s.TxtInput.Foreground = s.ZForeground;
            }
        }
        #endregion

        public event EventHandler<string>? ValueChanged;

        public bool IsReadonly { 
            get { return TxtInput.IsReadOnly; } 
            set { TxtInput.IsReadOnly = value;
                TxtInput.Background = IsReadonly ? Brushes.WhiteSmoke : Brushes.White;
            } }

        public GrSiloV1Part2()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetBrush(Brush b)
        {
            ImgSep.ZBlend = b;
            ImgBody.ZBlend = b;
        }

        public void SetForeground(Brush b)
        {
            TxtInput.Foreground = b;
        }

        private void TxtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_valueChanged)
            {
                ValueChanged?.Invoke(this, TxtInput.Text);
                _valueChanged = false;
            }
        }

        private void TxtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && _valueChanged)
            {
                ValueChanged?.Invoke(this, TxtInput.Text);
                _valueChanged = false;
            }
        }

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _valueChanged = true;
        }
    }
}
