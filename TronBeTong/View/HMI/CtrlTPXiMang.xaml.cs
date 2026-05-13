using System;
using System.Windows;
using System.Windows.Controls;
using TronBeTongV3.Comm;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlTPXiMang.xaml
    /// </summary>
    public partial class CtrlTPXiMang : UserControl
    {
        private const double _EPS = 0.01;

        #region ZNoTP
        public int ZNoTP
        {
            get { return (int)GetValue(ZNoTPProperty); }
            set { SetValue(ZNoTPProperty, value); }
        }
        public static readonly DependencyProperty ZNoTPProperty =
            DependencyProperty.Register("ZNoTP", typeof(int), typeof(CtrlTPXiMang), new PropertyMetadata(8, OnZNoTPPropertyChanged));
        private static void OnZNoTPPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlTPXiMang tp) tp.ShowTP();
        }
        #endregion

        private CtrlSilo02[] _tps = new CtrlSilo02[CauHinhTramTron.MAX_XM];
        private double[] updateThemBot = new double[CauHinhTramTron.MAX_XM];
        public ModelHeThong? TramTron { get; set; }

        public int MeDat { get; set; }
        public int MeMax { get { return Math.Max(WSCements1.MeHT, WSCements2.MeHT); } }

        public CtrlTPXiMang()
        {
            InitializeComponent();

            for (int i = 0; i < CauHinhTramTron.MAX_XM && i < PnlSilos.Children.Count; i++)
            {
                _tps[i] = (CtrlSilo02)PnlSilos.Children[i];
                _tps[i].Id = i;
                _tps[i].ButtonClicked += CtrlTPXiMang_ButtonClicked;
                _tps[i].BtVib.Visibility = Visibility.Collapsed;
                _tps[i].RoundDigit = 0;
                _tps[i].RoundFormat = "0";
            }

            WSCements1.LoaiCan = 0;
            WSCements2.LoaiCan = 0;
            WSCements1.SiloIndices.Add(0);
            WSCements1.SiloIndices.Add(1);
            WSCements2.SiloIndices.Add(2);
            WSCements2.SiloIndices.Add(3);
            WSCements1.DigitFormat = "0";
            WSCements2.DigitFormat = "0";
            WSCements1.ZShowDischargeTime = false;
            WSCements2.ZShowDischargeTime = false;
        }

        /// <summary>
        /// Load tham số từ DbSettings, gọi sau khi load DbSettings
        /// </summary>
        public void LoadThamSo(DbSettings s)
        {
            WSCements1.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.xi1.tre", 1);
            WSCements2.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.xi2.tre", 1);
        }

        /// <summary>
        /// Có thể chốt mẻ không?
        /// </summary>
        public bool CheckChotMe()
        {
            bool ret = false;
            if (TramTron != null)
            {
                if (WSCements1.CheckChotMe(TramTron.WIState.ChotXM))
                {
                    // Kiểm tra kl chốt cân 1
                    if (KiemTraKLChot(WSCements1))
                    {
                        //WSCements1.NeedSaveWeights = false;
                        WSCements1.IsSaveWeightsState = false;
                        ret |= true;
                    }
                }
                if (WSCements2.CheckChotMe(TramTron.WIState.ChotXM))
                {
                    // Kiểm tra kl chốt cân 2
                    if (KiemTraKLChot(WSCements2))
                    {
                        //WSCements2.NeedSaveWeights = false;
                        WSCements2.IsSaveWeightsState = false;
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
                if (i < TramTron.XMChots.Length) tklchot += TramTron.XMChots[i].Value;
            }
            return tklchot > 0;
        }

        /// <summary>
        /// Điền dữ liệu vào mẻ
        /// </summary>
        /// <param name="m"></param>
        /// <returns>True nếu có thực hiện điền</returns>
        public bool FillMe(DHMeDO m)
        {
            if (TramTron == null) return false;

            bool dienDL = false;
            if ((m.Flags & 2) == 2 || WSCements1.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WSCements1, m, 0);
            }

            if ((m.Flags & 2) == 2 || WSCements2.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WSCements2, m, 1);
            }
            return dienDL;
        }

        /// <summary>
        /// Điền dữ liệu chốt XM vào me theo cân
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
            if (me.STT == ws.MeHT && !me.ChotXMs[stt])
            {
                // thì chốt dữ liệu
                foreach (int i in ws.SiloIndices)
                {
                    if (i < TramTron.XMChots.Length)
                        me.KLXi[i] = Math.Round(TramTron.XMChots[i].Value, 3);
                }
                me.ChotXMs[stt] = true;
                dienDL = true;
            }
            ws.NeedSaveWeights = false;
            ws.DaChotKLVaoMe = true;

            return dienDL;
        }

        public void SetupBin(int i)
        {
            if (i < 0 || i >= CauHinhTramTron.MAX_XM) return;

            var ch = CauHinhTramTron.Instance.GetCauHinh(Core.LoaiThanhPhan.XiMang, i);
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

        public void UpdateView(double delta)
        {
            if (TramTron == null) return;

            for (int i = 0; i < ModelHeThong.SoXMReal; i++)
            {
                int some = (int)TramTron.MeSoMe.Value;

                if (TramTron.XMCapPhois[i].IsChanged || TramTron.MeM3Dat.IsChanged || TramTron.MeSoMe.IsChanged)
                    _tps[i].UpdateCapPhoi(TramTron.XMCapPhois[i].Value, TramTron.MeM3Dat.Value, some);

                if (TramTron.XMCPMes[i].IsChanged)
                    _tps[i].UpdateKLMe(TramTron.XMCPMes[i].Value, some);

                if (TramTron.XMChots[i].IsChanged)
                    _tps[i].UpdateCanThuc(TramTron.XMChots[i].Value);
                _tps[i].ZState = (int)TramTron.VanXMs[i].Value;
            }

            for (int i = 0; i < ModelHeThong.SoXMReal; i++)
            {
                double add = TramTron.XMAdds[i].Value;
                if (Math.Abs(updateThemBot[i] - add) > _EPS) {
                    _tps[i].UpdateDouble(3, add);
                    updateThemBot[i] = add;
                }
            }

            WSCements1.UpdateView(TramTron.WIState, TramTron.CanXMs[0], MeDat, delta);
            WSCements2.UpdateView(TramTron.WIState, TramTron.CanXMs[1], MeDat, delta);
        }

        #region Buttons

        private void CtrlTPXiMang_ButtonClicked(object? sender, ButtonArgs e)
        {
            if (e.Button == Core.ButtonTypes.CmnConfig)
            {
                GlobalUIEvent.Instance.RaiseEvent(sender, GlobalUIEventKinds.CauHinhXiMang, e.ObjectId);
            }
            else if (e.Button == Core.ButtonTypes.ThemBot)
            {
                TramTron?.S71200_WriteXMAdd(e.ObjectId, e.Value);
            }
        }

        private void WSCements1_ButtonClick(object sender, ButtonArgs e)
        {
        }
        #endregion

        private void ShowTP()
        {
            for (int i = 0; i < CauHinhTramTron.MAX_XM; i++)
            {
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
            if (WSCements1.TTCanHT == TramTron.WIState.DayXM) return 1;
            if (WSCements2.TTCanHT == TramTron.WIState.DayXM) return 2;
            return 0;
        }
    }
}
