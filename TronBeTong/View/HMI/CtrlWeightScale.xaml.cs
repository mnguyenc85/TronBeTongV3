using Org.BouncyCastle.Math.Field;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TronBeTongV3.Comm;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WeightScale.xaml
    /// </summary>
    public partial class CtrlWeightScale : UserControl
    {
        #region ZState
        public int ZState
        {
            get { return (int)GetValue(ZStateProperty); }
            set { SetValue(ZStateProperty, value); }
        }

        public static readonly DependencyProperty ZStateProperty =
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlWeightScale), new PropertyMetadata(3, OnZStatePropertyChanged));
        private static void OnZStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlWeightScale s)
            {
                s.SetState();
            }
        }
        #endregion

        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }

        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlWeightScale), new PropertyMetadata(null, OnZFillPropertyChanged));

        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlWeightScale s)
            {
                s.Img1.ZBlend = s.ZFill;
            }
        }
        #endregion

        #region ZText
        public string? ZText
        {
            get { return (string)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }
        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(CtrlWeightScale), new PropertyMetadata(null));
        #endregion

        #region ZShowDischargeTime
        public bool ZShowDischargeTime
        {
            get { return (bool)GetValue(ZShowDischargeTimeProperty); }
            set { SetValue(ZShowDischargeTimeProperty, value); }
        }
        public static readonly DependencyProperty ZShowDischargeTimeProperty =
            DependencyProperty.Register("ZShowDischargeTime", typeof(bool), typeof(CtrlWeightScale), new PropertyMetadata(true, OnZShowDischargeTimePropertyChanged));

        private static void OnZShowDischargeTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlWeightScale ws)
            {
                ws.PnlDischargeTime.Visibility = ws.ZShowDischargeTime? Visibility.Visible: Visibility.Collapsed;
            }
        }
        #endregion

        private int _meht, _meht0, _medat0;
        private int _ttcan, _ttcan0;
        public int MeHT { get { return _meht; } }
        /// <summary>
        /// Đánh dấu cân -> trạng thái chốt mẻ
        /// </summary>
        public bool IsSaveWeightsState { get; set; }
        private DateTime _waitSaveWeightsT0;
        /// <summary>
        /// Cần lưu mẻ
        /// </summary>
        public bool NeedSaveWeights { get; set; }

        public List<int> SiloIndices { get; private set; } = [];

        public int TTCanHT { get { return _ttcan; } }
        public int TTCan0 { get { return _ttcan0; } }

        public int Id { get; set; }
        public int LoaiCan { get; set; } = -1;
        public string DigitFormat { get; set; } = "0.0";
        public event EventHandler<ButtonArgs>? ButtonClicked;

        /// <summary>
        /// Thời gian trễ từ khi đạt trạng thái đủ để chốt kl (s)
        /// </summary>
        public double ChotKLTre { get; set; } = 1;

        /// <summary>
        /// Nếu quá số lần này thì ko chốt nữa
        /// </summary>
        public int ChotKLSoLanThuMax { get; set; } = 60;
        private int _chotKLSoLan = 0;

        public bool DaChotKLVaoMe { get; set; } = false;

        public CtrlWeightScale()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
            ZState = 0;
        }

        public bool CheckChotMe(int chotState)
        {
            if (_ttcan != _ttcan0 && _ttcan == chotState)
            {
                IsSaveWeightsState = true;
                _waitSaveWeightsT0 = DateTime.Now;
                _chotKLSoLan = 0;
                DaChotKLVaoMe = false;
            }

            if (IsSaveWeightsState) {
                var tdht = DateTime.Now;
                double et = (tdht - _waitSaveWeightsT0).TotalSeconds;

                if (et > ChotKLTre)
                {
                    NeedSaveWeights = true;
                    _chotKLSoLan++;
                    _waitSaveWeightsT0 = tdht;

                    //IsSaveWeightsState = false;
                }
                if (_chotKLSoLan > ChotKLSoLanThuMax) IsSaveWeightsState = false;
            }

            return NeedSaveWeights;
        }

        /// <summary>
        /// Update view: ZText, OutValve's state, Vib's state, LEDArrow
        /// </summary>
        public void UpdateView(string kl, string tt, double delta)
        {
            ZText = kl;
            LblTT.Text = tt;
        }

        public void UpdateView(WeightIndicatorState witype, ModelWeightTagGroup tags, int medat, double delta)
        {
            _ttcan0 = _ttcan;
            _ttcan = (int)tags.TrangThai.Value;
            _meht = (int)tags.MeHT.Value;
            ZText = tags.KL.Value.ToString(DigitFormat);
            LblTT.Text = witype.GetStrTTCan(_ttcan, LoaiCan);
            ZState = (int)tags.OutputValves.Value;
            Arrow1.Update();
            UpdateSoMe(_meht, medat);
        }



        private void UpdateSoMe(int meht, int medat)
        {
            if (meht != _meht0 || medat != _medat0)
            {
                LblMe.Text = $"{meht}/{medat}";
                _meht0 = meht;
                _medat0 = medat;
            }
        }

        private void SetState()
        {
            BtValve.ZState = ZState;
            Arrow1.SetState(ZState);
        }

        private void BtValve_ZStateChanged(object sender, int e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs() { 
                Button = Core.ButtonTypes.VanDauRa, 
                BtState = e, 
                ObjectId = Id 
            });
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                // TODO: shouldn't happen
                ZText = "";
            }
        }

        private void BtVib_ZStateChanged(object sender, int e)
        {
            ButtonClicked?.Invoke(this, new ButtonArgs() { 
                Button = Core.ButtonTypes.DamRung, 
                BtState = e, 
                ObjectId = Id
            });
        }
    }
}
