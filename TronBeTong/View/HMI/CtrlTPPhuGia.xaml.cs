using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TronBeTongV3.Comm;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlTPPhuGia.xaml
    /// Chưa sửa chốt dữ liệu
    /// </summary>
    public partial class CtrlTPPhuGia : UserControl
    {
        private const double _EPS = 0.01;

        #region ZNoTP
        public int ZNoTP
        {
            get { return (int)GetValue(ZNoTPProperty); }
            set { SetValue(ZNoTPProperty, value); }
        }
        public static readonly DependencyProperty ZNoTPProperty =
            DependencyProperty.Register("ZNoTP", typeof(int), typeof(CtrlTPPhuGia), new PropertyMetadata(8, OnZNoTPPropertyChanged));
        private static void OnZNoTPPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlTPPhuGia tp) tp.HienThiSilo();
        }
        #endregion

        #region ZShowWI
        public bool ZShowWI
        {
            get { return (bool)GetValue(ZShowWIProperty); }
            set { SetValue(ZShowWIProperty, value); }
        }
        public static readonly DependencyProperty ZShowWIProperty =
            DependencyProperty.Register("ZShowWI", typeof(bool), typeof(CtrlTPPhuGia), new PropertyMetadata(true, OnZShowWIPropertyChanged));
        private static void OnZShowWIPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlTPPhuGia c)
            {
                c.WScale1.Visibility = c.ZShowWI? Visibility.Visible: Visibility.Collapsed;
            }
        }
        #endregion

        public bool CanPGNgoai { get; set; }

        private CtrlSilo01[] _tps = new CtrlSilo01[8];
        public ModelHeThong? TramTron { get; set; }
        public int MeDat { get; set; }
        public int MeMax { get { return WScale1.MeHT; } }

        public CtrlTPPhuGia()
        {
            InitializeComponent();

            for (int i = 0; i < CauHinhTramTron.MAX_PG && i < PnlSilos.Children.Count; i++)
            {
                _tps[i] = (CtrlSilo01)PnlSilos.Children[i];
                _tps[i].Id = i;
                _tps[i].ButtonClicked += CtrlTPPhuGia_ButtonClicked;
            }

            WScale1.SiloIndices.Add(0);
            WScale1.SiloIndices.Add(1);
            WScale1.LoaiCan = 3;
            WScale1.ZShowDischargeTime = false;
        }

        public void LoadThamSo(DbSettings s)
        {
            WScale1.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.pg1.tre", 1);
        }

        /// <summary>
        /// Có thể chốt mẻ không?
        /// </summary>
        public bool CheckChotMe()
        {
            bool ret = false;
            if (TramTron != null)
            {
                if (WScale1.CheckChotMe(TramTron.WIState.ChotPG))
                {
                    // Kiểm tra kl chốt cân 1
                    if (KiemTraKLChot(WScale1))
                    {
                        //WScale1.NeedSaveWeights = false;
                        WScale1.IsSaveWeightsState = false;
                        ret |= true;
                    }
                }
            }
            return ret;
        }
        private bool KiemTraKLChot(CtrlWeightScale ws)
        {
            if (TramTron == null) return false;

            double tklchot = 0;
            foreach (int i in ws.SiloIndices)
            {
                if (i < TramTron.PGChots.Length) tklchot += TramTron.PGChots[i].Value;
            }
            return tklchot > 0;
        }

        /// <summary>
        /// Điền dữ liệu vào mẻ
        /// </summary>
        /// <param name="m"></param>
        public bool FillMe(DHMeDO m)
        {
            if (TramTron == null) return false;

            bool dienDL = false;
            // Nếu cân đủ hoặc dừng tay
            if ((m.Flags & 2) == 2 || WScale1.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WScale1, m, 0);
            }

            return dienDL;
        }

        /// <summary>
        /// Nếu điền dữ liệu vào mẻ nếu 'Cân PG ngoài'
        /// </summary>
        /// <param name="me"></param>
        public void FillMeByCP(DHMeDO me)
        {
            if (CanPGNgoai)
            {
                for (int i = 0; i < ModelHeThong.SoPGReal && i < me.KLPG.Length; i++)
                {
                    if (_tps[i].Visibility == Visibility.Visible)
                    {
                        me.KLPG[i] = _tps[i].KLMe;
                    }
                }
            }
        }

        /// <summary>
        /// Điền dữ liệu chốt phụ gia vào me theo cân
        /// </summary>
        /// <param name="ws">Cân</param>
        /// <param name="me">Mẻ</param>
        /// <param name="stt">STT của cân</param>
        /// <returns>true nếu có điền dữ liệu</returns>
        private bool FillMeByWI(CtrlWeightScale ws, DHMeDO me, int stt)
        {
            if (TramTron == null || ws.DaChotKLVaoMe) return false;

            bool dienDL = false;
            // Nếu stt của mẻ == mẻ hiện tại của cân
            if (me.STT == ws.MeHT && !me.ChotPGs[stt])
            {
                // thì chốt dữ liệu
                foreach (int i in ws.SiloIndices)
                {
                    if (i < TramTron.PGChots.Length)
                        me.KLPG[i] = Math.Round(TramTron.PGChots[i].Value, 3);
                }
                me.ChotPGs[stt] = true;
                dienDL = true;
            }
            ws.NeedSaveWeights = false;
            ws.DaChotKLVaoMe = true;

            return dienDL;
        }

        public void SetupBin(int i)
        {
            if (i < 0 || i >= CauHinhTramTron.MAX_PG) return;

            var ch = CauHinhTramTron.Instance.GetCauHinh(Core.LoaiThanhPhan.PhuGia, i);
            if (ch == null || ch.NguyenLieu == null)
            {
                _tps[i].ZTitle = null;
                _tps[i].ZEnabled = false;
            }
            else
            {
                _tps[i].ZTitle = ch.NguyenLieu.Ma;
                _tps[i].ZEnabled = true;

                //_bins[i].ZHasVib = ch.DamRung;
            }
        }

        public void Update(double delta)
        {
            if (TramTron == null) return;

            if (!CanPGNgoai)
            {
                for (int i = 0; i < ModelHeThong.SoPGReal; i++)
                {
                    if (_tps[i].Visibility == Visibility.Visible)
                    {
                        int some = (int)TramTron.MeSoMe.Value;

                        if (TramTron.PGCapPhois[i].IsChanged || TramTron.MeM3Dat.IsChanged || TramTron.MeSoMe.IsChanged)
                            _tps[i].UpdateCapPhoi(TramTron.PGCapPhois[i].Value, TramTron.MeM3Dat.Value, some);

                        if (TramTron.PGCPMes[i].IsChanged)
                            _tps[i].UpdateKLMe(TramTron.PGCPMes[i].Value, some);

                        if (TramTron.VanPGs[i].IsChanged)
                        {
                            if (TramTron.VanPGs[i].GetBool())
                                _tps[i].ZState = 1;
                            else
                                _tps[i].ZState = 0;
                        }
                    }
                }

                for (int i = 0; i < CauHinhTramTron.MAX_PG; i++)
                {
                    if (_tps[i].Visibility == Visibility.Visible)
                    {
                        _tps[i].SetChot(TramTron.PGChots[i].Value);
                    }
                }
            }

            WScale1.UpdateView(TramTron.WIState, TramTron.CanPGs[0], MeDat, delta);
        }

        public void SetCanPGNgoai(DHCongThucVM? ct, double m3, int some)
        {
            CanPGNgoai = true;
            PnlCanPGNgoai.Visibility = Visibility.Visible;
            for (int i = 0; i < ModelHeThong.SoPGReal; i++)
            {
                if (_tps[i].Visibility == Visibility.Visible)
                {
                    double klct = 0;
                    if (ct != null)
                    {
                        var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.PhuGia && x.NL_SiloIndex == i);
                        if (tp != null) klct = tp.KLCongThuc;
                    }
                   
                    _tps[i].UpdateCapPhoi(klct, 0, 0);
                    if (some > 0)
                        _tps[i].UpdateKLMe(klct * m3 / some, some);
                    else
                        _tps[i].UpdateKLMe(0, some);

                    _tps[i].SetColor(0, Brushes.OrangeRed);
                }
            }
        }
        public void SetCanPG()
        {
            CanPGNgoai = false;
            PnlCanPGNgoai.Visibility = Visibility.Collapsed;
            for (int i = 0; i < ModelHeThong.SoPGReal; i++)
            {
                if (_tps[i].Visibility == Visibility.Visible)
                {
                    _tps[i].SetColor(0, Brushes.Black);
                }
            }
        }

        #region Buttons
        private void CtrlTPPhuGia_ButtonClicked(object? sender, ButtonArgs e)
        {
            if (e.Button == Core.ButtonTypes.CmnConfig)
            {
                GlobalUIEvent.Instance.RaiseEvent(sender, GlobalUIEventKinds.CauHinhPhuGia, e.ObjectId);
            }
            else
            {
            }
        }

        private void WScale1_ButtonClick(object sender, ButtonArgs e)
        {
        }
        #endregion
        
        /// <summary>
        /// Ẩn/hiện silo
        /// </summary>
        private void HienThiSilo()
        {
            for (int i = 0; i < CauHinhTramTron.MAX_PG; i++) { 
                if (i < ZNoTP) _tps[i].Visibility = Visibility.Visible;
                else _tps[i].Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// Kiểm tra xem có cân nào ở trạng thái đầy không?
        /// </summary>
        /// <returns></returns>
        public int CheckCanDay()
        {
            if (TramTron == null) return 0;
            if (WScale1.TTCanHT == TramTron.WIState.DayPG) return 1;
            return 0;
        }
    }
}
