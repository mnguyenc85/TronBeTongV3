using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TronBeTongV3.Core;
using NMComm.S71200;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrWaterSilo.xaml
    /// </summary>
    public partial class CtrlWaterSilo : UserControl
    {
        #region ZTitle
        public string ZTitle
        {
            get { return (string)GetValue(ZTitleProperty); }
            set { SetValue(ZTitleProperty, value); }
        }
        public static readonly DependencyProperty ZTitleProperty =
            DependencyProperty.Register("ZTitle", typeof(string), typeof(CtrlWaterSilo), new PropertyMetadata(null));
        #endregion

        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }

        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlWaterSilo), new PropertyMetadata(null, OnZFillPropertyChanged));
        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlWaterSilo s)
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
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlWaterSilo), new PropertyMetadata(3, OnZStatePropertyChanged));
        private static void OnZStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlWaterSilo s)
            {
                s.SetState();
            }
        }
        #endregion

        public int Id { get; set; }
        public event EventHandler<ButtonArgs>? ButtonClicked;
        /// <summary>
        /// Số dấu phẩy thập phân
        /// </summary>
        public int RoundDigit { get; set; } = 1;
        public string RoundFormat { get; set; } = "0.0";

        public CtrlWaterSilo()
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

        public void UpdateCapPhoi(double capphoi, double m3, int some)
        {
            double cp = Math.Round(capphoi, RoundDigit);
            double tt = Math.Round(m3, RoundDigit);

            SiloPart1.ZText = cp.ToString(RoundFormat);
            //if (some > 0)
            //    SiloPart2.ZText = (cp * tt / some).ToString(RoundFormat);
            //else SiloPart2.ZText = "-";
        }

        public void UpdateCanThuc(double kl)
        {
            SiloPart3.ZText = kl.ToString(RoundFormat);
        }

        public void UpdateKLMe(double kl, int some)
        {
            if (some > 0) SiloPart2.ZText = kl.ToString(RoundFormat);
            else SiloPart2.ZText = "-";
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
            ImgConveyor.ZBlend = ZState > 0 ? MyBrushes.BrTransparentOn : MyBrushes.BrTransparentOff;
            BtPump.ZState = ZState;
        }

        private void BtPump_ZStateChanged(object sender, int e)
        {
            //ButtonClicked?.Invoke(this, new ButtonArgs()
            //{
            //    Button = ButtonTypes.DongCo,
            //    BtState = e,
            //    ObjectId = Id,
            //});
        }
    }
}
