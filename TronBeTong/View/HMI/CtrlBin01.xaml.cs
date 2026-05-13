using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NMComm.S71200;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlBin01.xaml
    /// TODO: Bỏ PlcTag khỏi Control này
    /// </summary>
    public partial class CtrlBin01 : UserControl
    {
        private const double EPSILON = 0.001;

        #region Dependency Properties
        #region ZTitle
        public string? ZTitle
        {
            get { return (string?)GetValue(ZTitleProperty); }
            set { SetValue(ZTitleProperty, value); }
        }
        public static readonly DependencyProperty ZTitleProperty =
            DependencyProperty.Register("ZTitle", typeof(string), typeof(CtrlBin01), new PropertyMetadata(null));
        #endregion

        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }
        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlBin01), new PropertyMetadata(null, OnZFillPropertyChanged));
        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlBin01 s)
            {
                s.GrBin1.SetBrush(s.ZFill);
            }
        }
        #endregion

        #region Valves
        public int ZOn1
        {
            get { return (int)GetValue(ZOn1Property); }
            set { SetValue(ZOn1Property, value); }
        }
        public static readonly DependencyProperty ZOn1Property =
            DependencyProperty.Register("ZOn1", typeof(int), typeof(CtrlBin01), new PropertyMetadata(0, OnZOnPropertyChanged));
        public int ZOn2
        {
            get { return (int)GetValue(ZOn2Property); }
            set { SetValue(ZOn2Property, value); }
        }
        public static readonly DependencyProperty ZOn2Property =
            DependencyProperty.Register("ZOn2", typeof(int), typeof(CtrlBin01), new PropertyMetadata(0, OnZOnPropertyChanged));
        private static void OnZOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlBin01 s)
            {
                s.GrBin1.UpdateView(s.ZOn1, 0);
            }
        }

        public double ZTimeOn1
        {
            get { return (double)GetValue(ZTimeOn1Property); }
            set { SetValue(ZTimeOn1Property, value); }
        }
        public static readonly DependencyProperty ZTimeOn1Property =
            DependencyProperty.Register("ZTimeOn1", typeof(double), typeof(CtrlBin01), new PropertyMetadata(0d, OnZTimeOnPropertyChanged));
        public double ZTimeOn2
        {
            get { return (double)GetValue(ZTimeOn2Property); }
            set { SetValue(ZTimeOn2Property, value); }
        }
        public static readonly DependencyProperty ZTimeOn2Property =
            DependencyProperty.Register("ZTimeOn2", typeof(double), typeof(CtrlBin01), new PropertyMetadata(0d, OnZTimeOnPropertyChanged));
        private static void OnZTimeOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlBin01 s)
            {
                s.GrBin1.SetTimeOn(s.ZTimeOn1);
            }
        }
        #endregion

        #region ZHasVib
        public bool ZHasVib
        {
            get { return (bool)GetValue(ZHasVibProperty); }
            set { SetValue(ZHasVibProperty, value); }
        }
        public static readonly DependencyProperty ZHasVibProperty =
            DependencyProperty.Register("ZHasVib", typeof(bool), typeof(CtrlBin01), new PropertyMetadata(true, OnZHasVibChanged));

        private static void OnZHasVibChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlBin01 s) s.BtVib.Visibility = s.ZHasVib ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region ZEnabled
        public bool ZEnabled
        {
            get { return (bool)GetValue(ZEnabledProperty); }
            set { SetValue(ZEnabledProperty, value); }
        }
        public static readonly DependencyProperty ZEnabledProperty =
            DependencyProperty.Register("ZEnabled", typeof(bool), typeof(CtrlBin01), new PropertyMetadata(true, OnZEnabledPropertyChanged));

        private static void OnZEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlBin01 b)
            {
                b.PnlMain.IsEnabled = b.ZEnabled;
            }
        }
        #endregion

        #endregion

        public event EventHandler<ButtonArgs>? ButtonClicked;
        public int Id { get; set; }

        /// <summary>
        /// Số dấu phẩy thập phân
        /// </summary>
        public int RoundDigit { get; set; } = 1;
        public string RoundFormat { get; set; } = "0.0";

        public CtrlBin01()
        {
            InitializeComponent();
            PnlMain.DataContext = this;

            GrBin1.UpdateView(0, 0);
            GrBin1.BtOn1.ZStateChanged += BtOn1_ZStateChanged;
        }

        public void UpdateView(double delta)
        {
            GrBin1.UpdateArrowView(delta);
        }

        public string GetText(int i)
        {
            switch (i)
            {
                case 0: return TxtField1.Text;
                case 1: return TxtField2.Text;
                case 2: return TxtField3.Text;
                case 3: return TxtField4.Text;
                case 4: return TxtField5.Text;
            }
            return "0";
        }

        public void UpdateCapPhoi(double capphoi, double m3, int some)
        {
            double cp = Math.Round(capphoi, RoundDigit);
            double tt = Math.Round(m3, RoundDigit);

            TxtField1.Text = cp.ToString(RoundFormat);
            //if (some > 0)
            //    TxtField2.Text = (cp * tt / some).ToString(RoundFormat);
            //else TxtField2.Text = "-";

            //TxtField3.Text = tags.CanThuc.Value.ToString("0.0");
            //TxtField4.Text = tags.HieuChinh.Value.ToString("0.0");
            //GrBin1.UpdateView((int)tags.Valve1.Value, (int)tags.Valve2.Value, delta);
            //BtVib.ZState = (int)tags.Vib.Value;
        }

        private double _humid = 0, _add = 0;
        public void UpdateHieuChinh(PlcTag HumidTag, PlcTag AddTag)
        {
            double h = Math.Round(HumidTag.Value, 1);
            double a = Math.Round(AddTag.Value, 1);
                
            if (Math.Abs(h - _humid) > EPSILON) {  
                _humid = h; TxtField4.SetValueNoEvent(h);
            }
            if (Math.Abs(a - _add) > EPSILON)
            {
                _add = h; TxtField5.SetValueNoEvent(h);
            }
        }

        public void UpdateKLMe(double kl, int some)
        {
            if (some > 0) TxtField2.Text = kl.ToString(RoundFormat);
            else TxtField2.Text = "-";
        }

        public void UpdateText(int i, double v)
        {
            if (i == 2)
            {
                TxtField2.Text = v.ToString(RoundFormat);
            }
            if (i == 3)
            {
                TxtField3.Text = v.ToString(RoundFormat);
            }
            else if (i == 4)
            {
                TxtField4.Text = v.ToString(RoundFormat);
            }
            else if (i == 5)
            {
                TxtField5.Text = v.ToString(RoundFormat);
            }
        }
        public void UpdateValue(int i, double v)
        {
            if (i == 4)
            {
                TxtField4.BlockInvoke = true;
                TxtField4.Value = v;
                TxtField4.Foreground = Brushes.Black;
                TxtField4.BlockInvoke = false;
            }
            else if (i == 5)
            {
                TxtField5.BlockInvoke = true;
                TxtField5.Value = v;
                TxtField5.Foreground = Brushes.Black;
                TxtField5.BlockInvoke = false;
            }
        }

        private void BtOn1_ZStateChanged(object? sender, int e)
        {
            //GrBin1.SetOn(e, -1);
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.VanDauRa,
                BtState = GrBin1.BtOn1.ZState * 2 + e,
                ObjectId = Id
            });
        }

        private void BtVib_ZStateChanged(object sender, int e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.DamRung,
                BtState = e,
                ObjectId = Id
            });
        }

        private void CmnConfig_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.CmnConfig,
                BtState = 0,
                ObjectId = Id
            });
        }

        private void TxtField5_ValueChanged(object sender, double e)
        {
            TxtField5.Foreground = Brushes.Red;
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.DoAm,
                BtState = 0,
                Value = e,
                ObjectId = Id
            });
        }

        private void TxtField4_ValueChanged(object sender, double e)
        {
            TxtField4.Foreground = Brushes.Red;
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.ThemBot,
                BtState = 0,
                Value = e,
                ObjectId = Id
            });
        }
    }
}
