using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TronBeTongV3.Core;
using NMComm.S71200;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrSilo02.xaml
    /// </summary>
    public partial class CtrlSilo02 : UserControl
    {
        #region Dependency Properties
        #region ZTitle
        public string? ZTitle
        {
            get { return (string)GetValue(ZTitleProperty); }
            set { SetValue(ZTitleProperty, value); }
        }
        public static readonly DependencyProperty ZTitleProperty =
            DependencyProperty.Register("ZTitle", typeof(string), typeof(CtrlSilo02), new PropertyMetadata(null));
        #endregion

        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }

        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlSilo02), new PropertyMetadata(null, OnZFillPropertyChanged));
        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo02 s)
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
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlSilo02), new PropertyMetadata(3, OnZStatePropertyChanged));
        private static void OnZStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo02 s)
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
            DependencyProperty.Register("ZHasVib", typeof(bool), typeof(CtrlSilo02), new PropertyMetadata(true, OnZHasVibChanged));

        private static void OnZHasVibChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo02 s) s.BtVib.Visibility = s.ZHasVib ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region ZEnabled
        public bool ZEnabled
        {
            get { return (bool)GetValue(ZEnabledProperty); }
            set { SetValue(ZEnabledProperty, value); }
        }
        public static readonly DependencyProperty ZEnabledProperty =
            DependencyProperty.Register("ZEnabled", typeof(bool), typeof(CtrlSilo02), new PropertyMetadata(true, OnZEnabledPropertyChanged));

        private static void OnZEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlSilo02 b) b.PnlMain.IsEnabled = b.ZEnabled;
        }
        #endregion
        #endregion

        public int Id { get; set; }
        public event EventHandler<ButtonArgs>? ButtonClicked;
        /// <summary>
        /// Số dấu phẩy thập phân
        /// </summary>
        public int RoundDigit { get; set; } = 1;
        public string RoundFormat { get; set; } = "0.0";

        public CtrlSilo02()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
            SiloPart4.TxtInput.ValueChanged += TxtInput_ValueChanged;
        }

        public string GetText(int i)
        {
            switch (i)
            {
                case 0: return SiloPart1.ZText;
                case 1: return SiloPart2.ZText;
                case 2: return SiloPart3.ZText;
                case 3: return SiloPart4.ZText;
            }
            return "0";
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
            if (some > 0) SiloPart2.ZText = kl.ToString(RoundFormat);
            else SiloPart2.ZText = "-";
        }

        public void UpdateCanThuc(double kl)
        {
            SiloPart3.ZText = kl.ToString(RoundFormat);
        }

        public void UpdateDouble(int i, double v)
        {
            if (i == 3)
            {
                SiloPart4.TxtInput.BlockInvoke = true;
                SiloPart4.TxtInput.Value = v;
                SiloPart4.ZForeground = Brushes.Black;
                SiloPart4.TxtInput.BlockInvoke = false;
            }
        }

        //public void SetView(PlcTag cp, long t)
        //{
        //    if (t > _viewT)
        //    {
        //        SiloPart1.ZText = cp.Value.ToString("0.0");
        //        SiloPart2.ZText = tags.CanThuc.Value.ToString("0.0");
        //        SiloPart3.ZText = tags.HieuChinh.Value.ToString("0.0");
        //        ZState = (int)tags.ScrewConveyor.Value;
        //        BtVib.ZState = (int)tags.Vib.Value;
        //        _viewT = t;
        //    }
        //}

        public void SetBrush(Brush b)
        {
            SiloPart1.SetBrush(b);
            SiloPart2.SetBrush(b);
            SiloPart3.SetBrush(b);
            SiloPart4.SetBrush(b);
            ImgRect1.ZBlend = b;
        }

        private void SetState()
        {
            ImgConveyor.ZBlend = ZState > 0? MyBrushes.BrTransparentOn: MyBrushes.BrTransparentOff;
            BtMotor.ZState = ZState;
        }

        private void BtMotor_ZStateChanged(object sender, int e)
        {
            // TEST:
            //ZState = e;
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = ButtonTypes.VanDauRa,
                BtState = e,
                ObjectId = Id,
            });
        }

        private void BtVib_ZStateChanged(object sender, int e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = ButtonTypes.DamRung,
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

        private void TxtInput_ValueChanged(object? sender, double e)
        {
            SiloPart4.ZForeground = Brushes.Red;
            ButtonClicked?.Invoke(this, new ButtonArgs()
            {
                Button = ButtonTypes.ThemBot,
                Value = e,
                BtState = 0,
                ObjectId = Id,
            });
        }
    }
}
