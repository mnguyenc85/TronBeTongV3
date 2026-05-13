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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for Ctrl7LED.xaml
    /// </summary>
    public partial class Ctrl7LED : UserControl
    {
        private List<Ctrl7LEDDigit> _digits = new();

        #region ZText
        public string ZText
        {
            get { return (string)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }

        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(Ctrl7LED), new PropertyMetadata(null, ZTextChanged));

        private static void ZTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LED c)
            {
                string? s = e.NewValue as string;
                if (s == null) c.Clear();
                else c.ShowText(s);
            }
        }
        #endregion

        #region ZShadowBrush
        public Brush ZShadowBrush
        {
            get { return (Brush)GetValue(ZShadowBrushProperty); }
            set { SetValue(ZShadowBrushProperty, value); }
        }

        public static readonly DependencyProperty ZShadowBrushProperty =
            DependencyProperty.Register("ZShadowBrush", typeof(Brush), typeof(Ctrl7LED), new PropertyMetadata(null, ZShadowBrushChanged));

        private static void ZShadowBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LED c)
            {
                Brush? b = e.NewValue as Brush;
                for (int i = 0; i < c._digits.Count; i++)
                    c._digits[i].ZShadowBrush = b;
            }
        }
        #endregion

        #region ZDigits
        public int ZDigits
        {
            get { return (int)GetValue(ZDigitsProperty); }
            set { SetValue(ZDigitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZDigits.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZDigitsProperty =
            DependencyProperty.Register("ZDigits", typeof(int), typeof(Ctrl7LED), new PropertyMetadata(6, ZDigitsChanged));

        private static void ZDigitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ctrl7LED c)
            {
                c.InitDigit((int)e.NewValue);
            }
        }
        #endregion

        private bool _initializing = false;

        public Ctrl7LED()
        {
            InitializeComponent();
            InitDigit(6);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitDigit(int n)
        {
            _initializing = true;
            PnlLED.Children.Clear();
            _digits.Clear();
            for (int i = 0; i < n; i++)
            {
                Ctrl7LEDDigit d = new()
                {
                    ZShadowBrush = this.ZShadowBrush
                };
                PnlLED.Children.Add(d);
                _digits.Add(d);
            }
            _initializing = false;
        }

        public void Clear()
        {
            if (_initializing) return;
            for (int i = 0; i < _digits.Count; i++)
                _digits[i].ZText = null;
        }

        private void ShowText(string s)
        {
            if (_initializing) return;

            int j = s.Length - 1;
            int i = _digits.Count - 1;
            while (i >= 0)
            {
                if (j >= 0)
                {
                    if (s[j] == '.')
                    {
                        if (--j >= 0)
                        {
                            _digits[i].ZText = $"{s[j]}.";
                        }
                    }
                    else
                    {
                        _digits[i].ZText = s[j].ToString();
                    }
                }
                else
                {
                    _digits[i].ZText = null;
                }
                i--;
                j--;
            }
        }
    }
}
