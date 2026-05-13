using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NMComm.S71200;
using TronBeTongV3.Core;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for SiloV1.xaml
    /// </summary>
    public partial class CtrlSilo01 : UserControl
    {
        #region Dependency Properties

        #region ZTitle
        public string? ZTitle
        {
            get { return (string)GetValue(ZTitleProperty); }
            set { SetValue(ZTitleProperty, value); }
        }
        public static readonly DependencyProperty ZTitleProperty =
            DependencyProperty.Register("ZTitle", typeof(string), typeof(CtrlSilo01), new PropertyMetadata(null));
        #endregion

        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }

        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlSilo01), new PropertyMetadata(null, OnZFillPropertyChanged));

        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo01 s)
            {
                s.SetBrush(s.ZFill);
            }
        }
        #endregion

        #region ZState
        public int ZState
        {
            get { return (int)GetValue(ZStateProperty); }
            set { SetValue(ZStateProperty, value); }
        }

        public static readonly DependencyProperty ZStateProperty =
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlSilo01), new PropertyMetadata(3, OnZStatePropertyChanged));
        private static void OnZStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo01 s)
            {
                s.SetState();
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
            DependencyProperty.Register("ZHasVib", typeof(bool), typeof(CtrlSilo01), new PropertyMetadata(false, OnZHasVibChanged));

        private static void OnZHasVibChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo01 s) s.BtVib.Visibility = s.ZHasVib? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region ZEnabled
        public bool ZEnabled
        {
            get { return (bool)GetValue(ZEnabledProperty); }
            set { SetValue(ZEnabledProperty, value); }
        }
        public static readonly DependencyProperty ZEnabledProperty =
            DependencyProperty.Register("ZEnabled", typeof(bool), typeof(CtrlSilo01), new PropertyMetadata(true, OnZEnabledPropertyChanged));

        private static void OnZEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo01 b)
            {
                b.PnlMain.IsEnabled = b.ZEnabled;
            }
        }
        #endregion
        #endregion

        //private long _viewT = 0;
        public int Id { get; set; }
        public event EventHandler<ButtonArgs>? ButtonClicked;
        /// <summary>
        /// Số dấu phẩy thập phân
        /// </summary>
        public int RoundDigit { get; set; } = 1;
        public string RoundFormat { get; set; } = "0.0";

        public double KLMe { get; set; }

        public CtrlSilo01()
        {
            InitializeComponent();
            PnlMain.DataContext = this;            
        }

        public string GetText(int i)
        {
            switch (i)
            {
                case 0: return SiloPart1.ZText;
                case 1: return SiloPart2.ZText;
                case 2: return SiloPart3.ZText;
            }
            return "0";
        }

        public void SetColor(int i, Brush br)
        {
            switch (i)
            {
                case 0: SiloPart1.SetForeground(br);
                    break;
            }
        }

        public void UpdateCapPhoi(double capphoi, double m3, int some)
        {
            double cp = Math.Round(capphoi, RoundDigit);
            double tt = Math.Round(m3, RoundDigit);

            SiloPart1.ZText = cp.ToString(RoundFormat);
            //if (some > 0)
            //    SiloPart2.ZText = (cp * tt / some).ToString("0.0");
            //else SiloPart2.ZText = "-";
        }
        public void UpdateKLMe(double kl, int some)
        {
            KLMe = kl;
            if (some > 0) SiloPart2.ZText = kl.ToString(RoundFormat);
            else SiloPart3.ZText = "-";
        }

        private double _chot0 = 0;
        public void SetChot(double v)
        {
            if (v != _chot0)
            {
                SiloPart3.ZText = v.ToString(RoundFormat);
                _chot0 = v;
            }
        }

        public void SetBrush(Brush b)
        {
            SiloPart1.SetBrush(b);
            SiloPart2.SetBrush(b);
            SiloPart3.SetBrush(b);
            ImgRect1.ZBlend = b;
        }

        private void SetState()
        {
            LEDArrow.SetState(ZState);
            BtValve.ZState = ZState;
        }

        private void BtValve_ZStateChanged(object sender, int e)
        {
            // TEST:
            //ZState = e;
            ButtonClicked?.Invoke(this, new ButtonArgs() { 
                Button = ButtonTypes.VanDauRa,
                BtState = e,
                ObjectId = Id,
            });
        }

        private void CmnConfig_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = ButtonTypes.CmnConfig,
                BtState = 0,
                ObjectId = Id,
            });
        }
    }
}
