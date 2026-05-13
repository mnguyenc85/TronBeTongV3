using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace NMWPFControls.Controls
{
    public class TextBoxInt : TextBox
    {
        #region ZValueMin
        public int ZValueMin
        {
            get { return (int)GetValue(ZValueMinProperty); }
            set { SetValue(ZValueMinProperty, value); }
        }

        public static readonly DependencyProperty ZValueMinProperty =
            DependencyProperty.Register("ZValueMin", typeof(int), typeof(TextBoxInt), new PropertyMetadata(int.MinValue));
        #endregion

        #region ZValueMax
        public int ZValueMax
        {
            get { return (int)GetValue(ZValueMaxProperty); }
            set { SetValue(ZValueMaxProperty, value); }
        }

        public static readonly DependencyProperty ZValueMaxProperty =
            DependencyProperty.Register("ZValueMax", typeof(int), typeof(TextBoxInt), new PropertyMetadata(int.MaxValue));
        #endregion

        private int _digit = 0;
        private StringBuilder sb = new();
        private int _val = 0, _lval = 0;
        private bool convert = true;
        private int _sdelta = 1, _ldelta = 10;
        private string _mask = "0";
        private EventArgs _nullEA = new();

        private bool _digitEn = false;

        public int Digit
        {
            get { return _digit; }
            set
            {
                if (value < 1) _digitEn = false;
                else
                {
                    if (_digit != value)
                    {
                        _digit = value;
                        _mask = new string('0', _digit);
                        Text = _val.ToString(_mask);
                    }
                }
            }
        }
        public int Value
        {
            get { return _val; }
            set { _val = value; CallValueChanged(); }
        }

        public event EventHandler<int>? ValueChanged;
        public event EventHandler? NextFocus;
        public event EventHandler? PrevFocus;

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (_digitEn)
            {
                int ci0 = CaretIndex;
                if (ci0 < _digit)
                {
                    sb.Clear();
                    int ci1 = ci0 + e.Text.Length;
                    if (ci1 > _digit) ci1 = _digit;
                    for (int i = 0; i < _digit; i++)
                    {
                        if (i >= ci0 && i < ci1) sb.Append(e.Text[i - ci0]);
                        else
                        {
                            if (i < Text.Length) sb.Append(Text[i]);
                        }
                    }
                    Text = sb.ToString();
                    CaretIndex = ci1;
                }
                else
                {
                    NextFocus?.Invoke(this, _nullEA);
                }
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Divide)
            {
                if (NextFocus != null)
                    NextFocus(this, _nullEA);
                else
                    CallValueChanged();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (_digitEn && CaretIndex >= _digit)
                {
                    NextFocus?.Invoke(this, _nullEA);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Left && CaretIndex == 0)
            {
                PrevFocus?.Invoke(this, _nullEA);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                _val += _sdelta;
                CallValueChanged();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                _val -= _sdelta;
                CallValueChanged();
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _val += _sdelta;
            }
            else if (e.Delta < 0)
            {
                _val -= _sdelta;
            }
            CallValueChanged();
            e.Handled = true;
            base.OnMouseWheel(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            CallValueChanged();
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (convert) int.TryParse(Text, out _val);
        }

        private void CallValueChanged()
        {
            if (_val < ZValueMin) _val = ZValueMin;
            if (_val > ZValueMax) _val = ZValueMax;

            if (_lval != _val)
            {
                ValueChanged?.Invoke(this, _val);
                _lval = _val;
            }
            convert = false;
            Text = _val.ToString(_mask);
            convert = true;
        }
    }
}
