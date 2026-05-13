using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for NumIntUpdown.xaml
    /// </summary>
    public partial class NumIntUpdown : UserControl
    {
        private DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Input);
        private int _lastValue = 0;
        private int mode = 0;
        private bool skipTextChanged = false;
        private Brush _actBorder;

        public int NumValue
        {
            get { return (int)GetValue(NumValueProperty); }
            set { SetValue(NumValueProperty, value); txtNum.Text = value.ToString(); }
        }
        public static readonly DependencyProperty NumValueProperty =
            DependencyProperty.Register("NumValue", typeof(int), typeof(NumIntUpdown), new PropertyMetadata(0, new PropertyChangedCallback(onNumValuePropertyChanged)));
        private static void onNumValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumIntUpdown niud)
            {
                niud.txtNum.Text = e.NewValue.ToString();
            }
        }

        public string StrValue
        {
            get { return NumValue.ToString(); }
            set
            {
                if (int.TryParse(value, out int v)) NumValue = v;
            }
        }
        public int Max { get; set; } = 10000;
        public int Min { get; set; } = 0;
        public int SmallChange { get; set; } = 1;
        public int LargeChange { get; set; } = 5;

        private int _udAlign = 0;
        public int UpDownAlign
        {
            get { return _udAlign; }
            set { if (_udAlign != value) { _udAlign = value;
                    if (_udAlign == 0)
                    {
                        DockPanel.SetDock(pnlUpDown, Dock.Right);
                        txtNum.HorizontalContentAlignment = HorizontalAlignment.Left;
                    }
                    else
                    {
                        DockPanel.SetDock(pnlUpDown, Dock.Left);
                        txtNum.HorizontalContentAlignment = HorizontalAlignment.Right;
                    }
                }
            }
        }
        public int FocusMode { get; set; } = 0;
        public Brush FocusBg { get; set; }
        public Thickness ContentMargin
        {
            get { return lbl1.Margin; }
            set { lbl1.Margin = value; border1.Margin = value; }
        }        

        public event EventHandler? ValueChanged = null;
        public event EventHandler<int>? OutBound = null;

        public string? Title
        {
            get { return lbl1.Content.ToString(); }
            set
            {
                lbl1.Content = value;
                //if (!string.IsNullOrEmpty(value)) lbl1.Width = 0;
            }
        }

        public void setValueBinding(object src, string path, bool whenPropChanged = false)
        {
            Binding b = new Binding()
            {
                Source = src,
                Path = new PropertyPath(path),
                Mode = BindingMode.TwoWay
            };
            if (whenPropChanged) b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            SetBinding(NumValueProperty, b);
        }

        public double RPadL { get; set; }
        public double RPadR { get; set; }
        public double ATitleW { get { return lbl1.Width; } set {lbl1.Width = value; } }
        public double RFieldW { get; set; }
        public double AFieldW { get { return border1.Width; } set { border1.Width = value; } }
        private double _uw = 10;
        public double UnitW
        {
            get { return _uw; }
            set
            {
                if (_uw != value && value > 0)
                {
                    _uw = value;
                    setLayout(_uw * RPadL, ATitleW, _uw * RFieldW - ATitleW, _uw * RPadR);
                }
            }
        }

        public void setRelWidth(double rpl, double atw, double rfw, double rpr)
        {
            RPadL = rpl; RPadR = rpr; ATitleW = atw; RFieldW = rfw;
        }

        /// <summary>
        /// DON'T USE THIS DIRECTLY
        /// </summary>
        public void setLayout(double wPadL, double wTit, double wVal, double wPadR)
        {
            if (wVal > 2) border1.Width = wVal;
            else border1.Width = double.NaN;
            //if (wTit > 0) lbl1.Width = wTit;
            //else lbl1.Width = double.NaN;
            Margin = new Thickness(wPadL, 0, wPadR, 0);
        }

        public NumIntUpdown()
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _timer.Tick += _timer_Tick;

            _actBorder = (Brush)FindResource("NMActiveBorderBrush");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            changeValue();
        }

        private void changeValue()
        {
            int v = NumValue;
            if (mode > 0 && v < Max)
            {
                if (mode < 10)
                {
                    mode++;
                    v += SmallChange;
                }
                else
                {
                    v += LargeChange;
                }
                if (v > Max) v = Max;
                NumValue = v;
            }
            else if (mode < 0 && v > Min)
            {
                if (mode > -10)
                {
                    mode--;
                    v -= SmallChange;
                }
                else
                {
                    v -= LargeChange;
                }
                if (v < Min) v = Min;
                NumValue = v;
            }
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null || skipTextChanged) return;
            skipTextChanged = true;

            int v = 0;
            if (string.IsNullOrWhiteSpace(txtNum.Text))
            {
                SetValue(NumValueProperty, v);
            }
            else
            {
                if (int.TryParse(txtNum.Text, out v))
                {
                    if (v > Max)
                    {
                        v = Max;
                    }
                    else if (v < Min) {
                        v = Min;
                    }
                    SetValue(NumValueProperty, v);
                }
            }

            skipTextChanged = false;
        }

        private void shpUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NumValue == Max)
            {
                OutBound?.Invoke(this, NumValue);
                return;
            }
            if (!_timer.IsEnabled)
            {
                shpUp.Fill = Brushes.DarkViolet;
                mode = 1;
                changeValue();              // instantly change
                _timer.Start();
            }
        }

        private void shpUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            shpUp.Fill = Brushes.Violet;
            callValueChanged();
        }

        private void shpUp_MouseLeave(object sender, MouseEventArgs e)
        {
            shpUp.Fill = Brushes.White;
            _timer.Stop();
            //callValueChanged();
        }

        private void shpUp_MouseEnter(object sender, MouseEventArgs e)
        {
            shpUp.Fill = Brushes.Violet;
        }

        private void shpDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NumValue == Min)
            {
                OutBound?.Invoke(this, NumValue);
                return;
            }
            shpDown.Fill = Brushes.DarkViolet;
            mode = -1;
            changeValue();
            _timer.Start();
        }

        private void shpDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            shpDown.Fill = Brushes.Violet;
            _timer.Stop();
            callValueChanged();
        }

        private void shpDown_MouseEnter(object sender, MouseEventArgs e)
        {
            shpDown.Fill = Brushes.Violet;
            _timer.Stop();
        }

        private void shpDown_MouseLeave(object sender, MouseEventArgs e)
        {
            shpDown.Fill = Brushes.White;
            _timer.Stop();
            //callValueChanged();
        }

        private void callValueChanged()
        {
            if (_lastValue != NumValue)
            {
                ValueChanged?.Invoke(this, new EventArgs());
                _lastValue = NumValue;
            }
        }

        private void txtNum_LostFocus(object sender, RoutedEventArgs e)
        {
            callValueChanged();
            txtNum.Text = NumValue.ToString();
            border1.BorderBrush = Brushes.LightGray;
            border1.Background = Brushes.White;
        }

        private void txtNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtNum.Text == "0")
            {
                txtNum.Text = "";
            }
            if (e.Key == Key.Enter)
            {
                callValueChanged();
            }            
        }

        private void txtNum_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_actBorder != null) border1.BorderBrush = _actBorder;
            if (FocusBg != null) border1.Background = FocusBg;
            shpDown.Fill = Brushes.White;
            shpUp.Fill = Brushes.White;
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                pnlContent.Background = Brushes.White;
                shpDown.Stroke = Brushes.Purple;
                shpUp.Stroke = Brushes.Purple;
            }
            else
            {
                pnlContent.Background = Brushes.WhiteSmoke;
                shpDown.Stroke = Brushes.LightGray;
                shpUp.Stroke = Brushes.LightGray;
                shpUp.Fill = Brushes.WhiteSmoke;
            }
        }
    }
}
