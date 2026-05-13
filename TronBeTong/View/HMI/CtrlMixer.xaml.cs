using System;
using System.Windows;
using System.Windows.Controls;
using TronBeTongV3.Comm;
using TronBeTongV3.Comm.S71200;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlMixer.xaml
    /// </summary>
    public partial class CtrlMixer : UserControl
    {
        private const double ROTATE_SPD = 50.0;

        #region HandAngle
        public double HandAngle
        {
            get { return (double)GetValue(HandAngleProperty); }
            set { SetValue(HandAngleProperty, value); }
        }
        public static readonly DependencyProperty HandAngleProperty =
            DependencyProperty.Register("HandAngle", typeof(double), typeof(CtrlMixer), new PropertyMetadata(0d));
        #endregion

        #region ZState
        public int ZState
        {
            get { return (int)GetValue(ZStateProperty); }
            set { SetValue(ZStateProperty, value); }
        }
        public static readonly DependencyProperty ZStateProperty =
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlMixer), new PropertyMetadata(0));
        #endregion

        #region ZOutState
        public int ZOutState
        {
            get { return (int)GetValue(ZOutStateProperty); }
            set { SetValue(ZOutStateProperty, value); }
        }
        public static readonly DependencyProperty ZOutStateProperty =
            DependencyProperty.Register("ZOutState", typeof(int), typeof(CtrlMixer), new PropertyMetadata(3, OnZOutStatePropertyChanged));
        private static void OnZOutStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlMixer m)
            {
                m.SetOutState();
            }    
        }
        #endregion

        public ModelHeThong? TramTron { get; set; }
        public int MixSetTime { get; private set; }
        public int DischargeSetTime { get; private set; }
        public int DischargeHalfSetTime { get; private set; }

        public CtrlMixer()
        {
            InitializeComponent();
            PnlMain.DataContext = this;

            ZOutState = 0;
        }

        public void Update(double delta)
        {
            if (TramTron == null) return;

            BtMotor.ZState = TramTron.MixerRunning.GetBool()? 2 : 0;
            LEDDCT.IsOn = TramTron.SensorMixerClose.GetBool();
            LEDMCTHalf.IsOn = TramTron.SensorMixerOpenHalf.GetBool();
            LEDMCT.IsOn = TramTron.SensorMixerOpen.GetBool();
            ZOutState = TramTron.MixerDischarge.GetBool()? 2 : 0;

            if (TramTron.MixerStart.IsChanged)
            {
                LblTGTron.Visibility = TramTron.MixerStart.GetBool() ? Visibility.Visible: Visibility.Hidden;
            }

            if (TramTron.MixerSetTGTron.IsChanged)
                MixSetTime = (int)TramTron.MixerSetTGTron.Value;
            if (TramTron.MixerSetTGXaNua.IsChanged)
                DischargeHalfSetTime = (int)TramTron.MixerSetTGXaNua.Value;
            if (TramTron.MixerSetTGXa.IsChanged)
                DischargeSetTime = (int)TramTron.MixerSetTGXa.Value;

            LblTGTron.Content = $"{TramTron.MixerTGTron.Value / 10}/{MixSetTime} s";
            LblTGXaNua.Content = $"{TramTron.MixerTGXaNua.Value}/{DischargeHalfSetTime}";
            LblTGXa.Content = $"{TramTron.MixerTGXa.Value}/{DischargeSetTime}";

            if (BtMotor.ZState > 0)
            {
                double a = HandAngle + ROTATE_SPD * delta * BtMotor.ZState;
                if (a > 360) a -= 360;
                HandAngle = a;
            }

            if (TramTron.MeSoMe.IsChanged || TramTron.MixerMeHt.IsChanged)
            {
                LblMeCan.Content = $"{TramTron.MixerMeHt.Value:F0}/{TramTron.MeSoMe.Value:F0}";
            }
        }

        private void SetOutState()
        {
            Arrow2.SetState(ZOutState);
            BtXaBeTong.ZState = ZOutState;
        }

        private void BtMotor_ZStateChanged(object sender, int e)
        {
            //ZState = e;
        }

        private void CtrlMultiStateButton_ZStateChanged(object sender, int e)
        {
            //ZOutState = e;
        }
    }
}
