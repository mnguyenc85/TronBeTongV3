using S7.Net;
using System;
using System.Windows;
using System.Windows.Controls;
using NMComm.S71200;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.Comm;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlTPCotLieu2.xaml
    /// </summary>
    public partial class CtrlTPCotLieu2 : UserControl
    {
        private const double _EPS = 0.01;
        #region ZSoTP
        public int ZSoTP
        {
            get { return (int)GetValue(ZSoTPProperty); }
            set { SetValue(ZSoTPProperty, value); }
        }

        public static readonly DependencyProperty ZSoTPProperty =
            DependencyProperty.Register("ZSoTP", typeof(int), typeof(CtrlTPCotLieu2), new PropertyMetadata(6, ZSoTPPropertyChanged));

        private static void ZSoTPPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlTPCotLieu2 c) c.HienThiThanhPhan();
        }
        #endregion

        private CtrlBin01[] _tps = new CtrlBin01[CauHinhTramTron.MAX_CL];
        private double[] updateDoAm = new double[CauHinhTramTron.MAX_CL];
        private double[] updateThemBot = new double[CauHinhTramTron.MAX_CL];
        public int MeDat { get; set; }
        public int MeMax { get { return Math.Max(Math.Max(WsTotalCL1.MeHT, WsTotalCL2.MeHT), WsTotalCL3.MeHT); } }
        public ModelHeThong? TramTron { get; set; }

        public CtrlTPCotLieu2()
        {
            InitializeComponent();

            for (int i = 0; i < CauHinhTramTron.MAX_CL; i++) {
                _tps[i] = (CtrlBin01)PnlSilos.Children[i];
                _tps[i].Id = i;
                _tps[i].ButtonClicked += BinCotLieu_ButtonClicked;
                _tps[i].RoundDigit = 0;
                _tps[i].RoundFormat = "0";
                updateDoAm[i] = 0;
                updateThemBot[i] = 0;

                System.Diagnostics.Debug.WriteLine($"{i} -> {_tps[i].Name}");
            }

            WsTotalCL1.SiloIndices.Add(0);
            WsTotalCL1.SiloIndices.Add(1);
            WsTotalCL1.SiloIndices.Add(2);
            //WsTotalCL3.SiloIndices.Add(3);
            WsTotalCL1.ZShowDischargeTime = false;
            WsTotalCL2.ZShowDischargeTime = false;
            WsTotalCL3.ZShowDischargeTime = false;

            WsTotalCL1.LoaiCan = 1;
            WsTotalCL2.LoaiCan = 1;
            WsTotalCL3.LoaiCan = 1;
            WsTotalCL1.DigitFormat = "0";
            WsTotalCL2.DigitFormat = "0";
            WsTotalCL3.DigitFormat = "0";
        }

        public void LoadThamSo(DbSettings s)
        {
            WsTotalCL1.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.cl1.tre", 1);
            WsTotalCL2.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.cl2.tre", 1);
            WsTotalCL3.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.cl3.tre", 1);
        }

        /// <summary>
        /// Có thể chốt mẻ không?
        /// </summary>
        public bool CheckChotMe()
        {
            bool ret = false;
            if (TramTron != null)
            {
                if (WsTotalCL1.CheckChotMe(TramTron.WIState.ChotCoLieu))
                {
                    // Kiểm tra kl chốt cân 1
                    if (KiemTraKLChot(WsTotalCL1))
                    {
                        //WsTotalCL1.NeedSaveWeights = false;                        
                        WsTotalCL1.IsSaveWeightsState = false;
                        ret |= true;
                    }
                }
                if (WsTotalCL2.CheckChotMe(TramTron.WIState.ChotCoLieu))
                {
                    // Kiểm tra kl chốt cân 2
                    if (KiemTraKLChot(WsTotalCL2))
                    {
                        //WsTotalCL2.NeedSaveWeights = false;
                        WsTotalCL2.IsSaveWeightsState = false;
                        ret |= true;
                    }
                }
                if (WsTotalCL3.CheckChotMe(TramTron.WIState.ChotCoLieu))
                {
                    // Kiểm tra kl chốt cân 3
                    if (KiemTraKLChot(WsTotalCL3))
                    {
                        //WsTotalCL3.NeedSaveWeights = false;
                        WsTotalCL3.IsSaveWeightsState = false;
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
                if (i < TramTron.ClChots.Length) tklchot += TramTron.ClChots[i].Value;
            }
            return tklchot > 0;
        }

        /// <summary>
        /// Điền dữ liệu vào mẻ
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true nếu có điền dữ liệu</returns>
        public bool FillMe(DHMeDO m)
        {
            if (TramTron == null) return false;

            bool dienDL = false;
            // Nếu cân đủ hoặc dừng tay cân 1
            if ((m.Flags & 2) == 2 || WsTotalCL1.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WsTotalCL1, m, 0);
            }

            // Nếu cân đủ hoặc dừng tay cân 2
            if ((m.Flags & 2) == 2 || WsTotalCL2.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WsTotalCL2, m, 1);
            }

            // Nếu cân đủ hoặc dừng tay cân 3
            if ((m.Flags & 2) == 2 || WsTotalCL3.NeedSaveWeights)
            {
                dienDL |= FillMeByWI(WsTotalCL3, m, 2);
            }

            return dienDL;
        }

        /// <summary>
        /// Điền dữ liệu chốt cốt liệu vào me theo cân
        /// </summary>
        /// <param name="ws">Cân</param>
        /// <param name="me">Mẻ</param>
        /// <param name="stt">STT của cân</param>
        /// <returns>true nếu điền dữ liệu</returns>
        private bool FillMeByWI(CtrlWeightScale ws, DHMeDO me, int stt)
        {            
            if (TramTron == null || ws.DaChotKLVaoMe) return false;

            bool dienDL = false;
            // Nếu stt của mẻ == mẻ hiện tại của cân
            if (me.STT == ws.MeHT && !me.ChotCotLieus[stt])
            {
                // thì chốt dữ liệu
                foreach (int i in ws.SiloIndices)
                {
                    if (i < TramTron.ClChots.Length)
                    {
                        me.KLCL[i] = Math.Round(TramTron.ClChots[i].Value, 3);
                        me.HuCL[i] = Math.Round(TramTron.ClDoAms[i].Value, 3);
                    }
                }
                me.ChotCotLieus[stt] = true;
                dienDL = true;
            }
            ws.NeedSaveWeights = false;
            ws.DaChotKLVaoMe = true;

            return dienDL;
        }

        public void SetupBin(int i)
        {
            if (i < 0 || i >= CauHinhTramTron.MAX_CL) return;

            var ch = CauHinhTramTron.Instance.GetCauHinh(Core.LoaiThanhPhan.CotLieu, i);
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

            for (int i = 0; i < CauHinhTramTron.MAX_CL; i++)
            {
                if (_tps[i].Visibility == Visibility.Visible)
                {
                    int some = (int)TramTron.MeSoMe.Value;

                    if (TramTron.CLCapPhois[i].IsChanged || TramTron.MeM3Dat.IsChanged || TramTron.MeSoMe.IsChanged)
                        _tps[i].UpdateCapPhoi(TramTron.CLCapPhois[i].Value, TramTron.MeM3Dat.Value, some);

                    if (TramTron.CLCPMes[i].IsChanged)
                    {
                        _tps[i].UpdateKLMe(TramTron.CLCPMes[i].Value, some);
                    }

                    if (TramTron.VanCLs[i].IsChanged || TramTron.VanTinhCLs[i].IsChanged)
                    {
                        if (TramTron.VanCLs[i].GetBool())
                            _tps[i].ZOn1 = 2;
                        else if (TramTron.VanTinhCLs[i].GetBool())
                            _tps[i].ZOn1 = 1;
                        else
                            _tps[i].ZOn1 = 0;
                    }

                    double doam = TramTron.ClDoAms[i].Value;
                    if (Math.Abs(updateDoAm[i] - doam) > _EPS)
                    {
                        _tps[i].UpdateValue(5, doam);
                        updateDoAm[i] = doam;
                    }

                    double add = TramTron.ClAdds[i].Value;
                    if (Math.Abs(updateThemBot[i] - add) > _EPS)
                    {
                        _tps[i].UpdateValue(4, add);
                        updateThemBot[i] = add;
                    }

                    _tps[i].UpdateText(3, TramTron.ClChots[i].Value);

                    _tps[i].UpdateView(delta);
                }
            }

            WsTotalCL1.UpdateView(TramTron.WIState, TramTron.CanCLs[0], MeDat, delta);
            WsTotalCL2.UpdateView(TramTron.WIState, TramTron.CanCLs[1], MeDat, delta);
            WsTotalCL3.UpdateView(TramTron.WIState, TramTron.CanCLs[2], MeDat, delta);
        }

        private void BinCotLieu_ButtonClicked(object? sender, ButtonArgs e)
        {
            if (e.Button == Core.ButtonTypes.CmnConfig)
            {
                GlobalUIEvent.Instance.RaiseEvent(sender, GlobalUIEventKinds.CauHinhCotLieu, e.ObjectId);
            }
            else if (e.Button == Core.ButtonTypes.ThemBot)
            {
                TramTron?.S71200_WriteCLAdd(e.ObjectId, e.Value);
            }
            else if (e.Button == Core.ButtonTypes.DoAm)
            {
                TramTron?.S71200_WriteCLHum(e.ObjectId, e.Value);
            }
        }
        /// <summary>
        /// Ẩn/hiển thị thành phần
        /// </summary>
        private void HienThiThanhPhan()
        {
            for (int i = 0; i < CauHinhTramTron.MAX_CL; i++)
            {
                if (i < ZSoTP) _tps[i].Visibility = Visibility.Visible;
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
            if (WsTotalCL1.TTCanHT == TramTron.WIState.DayCotLieu) return 1;
            if (WsTotalCL2.TTCanHT == TramTron.WIState.DayCotLieu) return 2;
            if (WsTotalCL3.TTCanHT == TramTron.WIState.DayCotLieu) return 3;
            //if (WsTotalCL4.TTCanHT == TramTron.WIState.DayCotLieu) return true;
            return 0;
        }
    }
}
