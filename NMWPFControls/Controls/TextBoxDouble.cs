using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace NMWPFControls.Controls
{
    public class TextBoxDouble: TextBox
    {
        #region ZValueMin
        public double ZValueMin
        {
            get { return (double)GetValue(ZValueMinProperty); }
            set { SetValue(ZValueMinProperty, value); }
        }

        public static readonly DependencyProperty ZValueMinProperty =
            DependencyProperty.Register("ZValueMin", typeof(double), typeof(TextBoxDouble), new PropertyMetadata(double.MinValue));
        #endregion

        #region ZValueMax
        public double ZValueMax
        {
            get { return (double)GetValue(ZValueMaxProperty); }
            set { SetValue(ZValueMaxProperty, value); }
        }

        public static readonly DependencyProperty ZValueMaxProperty =
            DependencyProperty.Register("ZValueMax", typeof(double), typeof(TextBoxDouble), new PropertyMetadata(double.MaxValue));
        #endregion

        public bool BlockInvoke { get; set; }

        private double _val = 0, _lval = 0;
        private bool convert = true;

        public double Delta { get; set; } = 1;

        public double Snap { get; set; } = 10;

        public double Value
        {
            get { return _val; }
            set { _val = value; callValueChanged(); }
        }

        public event EventHandler<double>? ValueChanged;

        public void SetValueNoEvent(double v)
        {
            _val = v;
            convert = false;
            Text = _val.ToString();
            convert = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                _val += Delta;
                callValueChanged();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                _val -= Delta;
                callValueChanged();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                callValueChanged();
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                {
                    _val += Snap;
                }
                else if (e.Delta < 0)
                {
                    _val -= Snap;
                }
                callValueChanged(true);
            }
            else
            {
                if (e.Delta > 0)
                {
                    _val += Delta;
                }
                else if (e.Delta < 0)
                {
                    _val -= Delta;
                }
                callValueChanged();
            }
            e.Handled = true;
            
            base.OnMouseWheel(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            callValueChanged();
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (convert)
            {
                if (double.TryParse(Text, out double v)) {
                    _val = v;
                }
            }
        }

        private void callValueChanged(bool bSnap = false)
        {
            if (bSnap && Snap > 0)
            {
                _val = Math.Round(_val / Snap) * Snap;
            }
            if (_val < ZValueMin) _val = ZValueMin;
            if (_val > ZValueMax) _val = ZValueMax;

            if (_lval != _val)
            {
                if (!BlockInvoke) ValueChanged?.Invoke(this, _val);
                _lval = _val;
            }
            convert = false;
            Text = _val.ToString();
            convert = true;
        }
    }
}
