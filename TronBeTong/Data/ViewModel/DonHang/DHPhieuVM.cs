using NMWPFControls.Core.MVVM;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.ViewModel.DonHang
{
    public class DHPhieuVM: VMBase
    {
        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        public int Id { get; set; }

        public DHDonVM? DonHang { get; set; }

        private string? _sophieu;
        public string? SoPhieu { get { return _sophieu; } set { if (_sophieu != value) { _sophieu = value; NotifyChanged(); } } }

        private KDXeVM? _xe;
        public KDXeVM? Xe { get { return _xe; } set { if (_xe != value) { _xe = value; NotifyChanged(); } } }

        private KDLaiXeVM? _lx;
        public KDLaiXeVM? LaiXe { get { return _lx; } set { if (_lx != value) { _lx = value; NotifyChanged(); } } }

        private DHCongThucVM? _ct;
        public DHCongThucVM? CongThuc { get { return _ct; } set { if (_ct != value) { _ct = value; NotifyChanged(); } } }

        private int _medat;
        public int MeDat { get { return _medat; } set { if (_medat != value) { _medat = value; NotifyChanged(); } } }

        private int _meht;
        public int MeHT { get { return _meht; } set { if (_meht != value) { _meht = value; NotifyChanged(); } } }

        private double _klDat;
        public double KLDat  { get { return _klDat; } set { if (_klDat != value) { _klDat = value; NotifyChanged(); } } }

        private double _klht;
        public double KLHT { get { return _klht; } set { if (_klht != value) { _klht = value; NotifyChanged(); } } }

        private double _thetichdat;
        public double TheTichDat { get { return _thetichdat; } set { if (_thetichdat != value) { _thetichdat = value; NotifyChanged(); } } }

        private double _thetichht;
        public double TheTichHT { get { return _thetichht; } set { if (_thetichht != value) { _thetichht = value; NotifyChanged(); } } }

        private double _thetichtichluy;
        public double TheTichTichLuy { get { return _thetichtichluy; } set { if (_thetichtichluy != value) { _thetichtichluy = value; NotifyChanged(); } } }

        private DateTime? _tgbd;
        public DateTime? TGBD { get { return _tgbd; } set { if (_tgbd != value) { _tgbd = value; NotifyChanged(); } } }
        private DateTime? _tght;
        public DateTime? TGHT { get { return _tght; } set { if (_tght != value) { _tght = value; NotifyChanged(); } } }

        private double _dongia;
        public double DonGia { get { return _dongia; } set { if (_dongia != value) { _dongia = value; NotifyChanged(); } } }

        private int _donStt;
        public int DonStt { get { return _donStt; } set { if (_donStt != value) { _donStt = value; NotifyChanged(); } } }
        private double _donm3;
        public double DonM3 { get { return _donm3; } set { if (_donm3 != value) { _donm3 = value; NotifyChanged(); } } }

        private int _donSttSim;
        public int DonSttSim { get { return _donSttSim; } set { if (_donSttSim != value) { _donSttSim = value; NotifyChanged(); } } }
        private double _donm3sim;
        public double DonM3Sim { get { return _donm3sim; } set { if (_donm3sim != value) { _donm3sim = value; NotifyChanged(); } } }

        private string? _tt;
        public string? StrTrangThai { get { return _tt; } set { if (_tt != value) { _tt = value; NotifyChanged(); } } }

        private double _ttme;
        public double TheTichMe { get { return _ttme; } set { if (_ttme != value) { _ttme = value; NotifyChanged(); } } }

        /// <summary>
        /// 128: Cân PG ngoài
        /// </summary>
        public int Flags { get; set; }

        public double[] TongKLTheoSilos = new double[19];

        private string? _kepchi;
        public string? KepChi { get { return _kepchi; } set { if (_kepchi != value) { _kepchi = value; NotifyChanged(); } } }

        private bool _canPGNgoai;
        public bool CanPGNgoai { get { return _canPGNgoai; } 
            set { if (_canPGNgoai != value) { 
                    _canPGNgoai = value; 
                    if (_canPGNgoai) Flags |= 128; else Flags &= ~128; 
                    NotifyChanged(); 
                } } }

        public void UpdateTrangThai()
        {
            if (Id <= 0) StrTrangThai = "Chuẩn bị";
            else
            {
                if (MeHT < MeDat)
                    StrTrangThai = $"{MeHT}/{MeDat}";
                else
                    StrTrangThai = "Hoàn thành";
            }
        }

        /// <summary>
        /// Kiểm tra TheTichHT, KLHT, MeHT so với dữ liệu mẻ
        /// </summary>
        public bool KiemTraThongTin(List<DHMeDO> dsme)
        {
            double eps = 0.001;

            int tongme = dsme.Count;
            double tongkl = 0;
            double tongm3 = 0;
            for (int j = 0; j < TongKLTheoSilos.Length; j++) TongKLTheoSilos[j] = 0;
            for (int i = 0; i < tongme; i++)
            {
                var me = dsme[i];
                tongkl += me.TongKL();
                tongm3 += me.M3Tron;
                
                for (int j = 0; j < me.KLCL.Length; j++)
                    TongKLTheoSilos[j] += me.KLCL[j];
                for (int j = 0; j < me.KLXi.Length; j++)
                    TongKLTheoSilos[5 + j] += me.KLXi[j];
                for (int j = 0; j < me.KLPG.Length; j++)
                    TongKLTheoSilos[10 + j] += me.KLPG[j];
                TongKLTheoSilos[18] += me.KLNuoc;
            }

            bool ismatch = true;
            if (MeHT != tongme)
            {
                MeHT = tongme;
                ismatch = false;
            }
            if (Math.Abs(KLHT - tongkl) > eps)
            {
                KLHT = Math.Round(tongkl);
                ismatch = false;
            }
            if (Math.Abs(TheTichHT - tongm3) > eps)
            {
                TheTichHT = Math.Round(tongm3, 1);
                ismatch = false;
            }

            return ismatch;
        }

        public void CopyFrom(DHPhieuVM o)
        {
            Id = o.Id;
            SoPhieu = o.SoPhieu;
            Xe = o.Xe;
            LaiXe = o.LaiXe;
            CongThuc = o.CongThuc;
            MeDat = o.MeDat;
            MeHT = o.MeHT;
            KLDat = o.KLDat;
            KLHT = o.KLHT;
            TheTichDat = o.TheTichDat;
            TheTichHT = o.TheTichHT;
            TGBD = o.TGBD;
            TGHT = o.TGHT;
            StrTrangThai = o.StrTrangThai;
            TheTichMe = o.TheTichMe;

            DonGia = o.DonGia;
            
            DonStt = o.DonStt;
            DonM3 = o.DonM3;
            DonSttSim = o.DonSttSim;
            DonM3Sim = o.DonM3Sim;

            KepChi = o.KepChi;

            Flags = o.Flags;
        }

        public DHPhieuVM Clone()
        {
            return new DHPhieuVM()
            {
                Id = Id,
                SoPhieu = SoPhieu,
                Xe = Xe,
                LaiXe = LaiXe,
                CongThuc = CongThuc,
                MeDat = MeDat,
                MeHT = MeHT,
                KLDat = KLDat,
                KLHT = KLHT,
                TheTichDat = TheTichDat,
                TheTichHT = TheTichHT,

                TGBD = TGBD,
                TGHT = TGHT,

                StrTrangThai = StrTrangThai,
                TheTichMe = TheTichMe,

                DonGia = DonGia,

                DonStt = DonStt,
                DonM3 = DonM3,
                DonSttSim = DonSttSim,
                DonM3Sim = DonM3Sim,

                KepChi = KepChi,

                Flags = Flags
            };
        }

        public void ToDO(DHPhieuDO o)
        {
            o.Id = Id;
            o.DonHangId = DonHang != null ? DonHang.Id : -1;
            o.SoPhieu = SoPhieu;
            o.XeId = Xe != null ? Xe.Id : -1;
            o.LaiXeId = LaiXe != null ? LaiXe.Id : -1;
            o.CongThucId = CongThuc != null ? CongThuc.Id : -1;
            o.MeDat = MeDat;
            o.KLDat = KLDat;
            o.TheTichDat = TheTichDat;
            o.MeHT = MeHT;
            o.KLHT = Math.Round(KLHT, 1);
            o.TheTichHT = Math.Round(TheTichHT, 1);

            o.TGBD = TGBD;
            o.TGHT = TGHT;
            o.TrangThai = Flags;

            o.DonGia = DonGia;
            
            o.DonM3 = DonM3;
            o.DonStt = DonStt;
            o.DonSttSim = DonSttSim;
            o.DonM3Sim = DonM3Sim;

            o.KepChi = KepChi;
        }

        public void FromDO(DHPhieuDO o)
        {
            Id = o.Id;
            SoPhieu = o.SoPhieu;

            // DonHang
            // Xe
            // LaiXe
            // CongThuc

            MeDat = o.MeDat;
            MeHT = o.MeHT;
            KLDat = o.KLDat;
            KLHT = Math.Round(o.KLHT);
            TheTichDat = o.TheTichDat;
            TheTichHT = Math.Round(o.TheTichHT, 3);

            TGBD = o.TGBD;
            TGHT = o.TGHT;

            DonGia = o.DonGia;

            DonStt = o.DonStt;
            DonM3 = o.DonM3;
            DonSttSim = o.DonSttSim;
            DonM3Sim = o.DonM3Sim;

            KepChi = o.KepChi;

            Flags = o.TrangThai;

            UpdateTrangThai();
            TheTichMe = MeDat > 0? TheTichDat / MeDat : 0;
        }

        public DHPhieuDO CreateDO()
        {
            var o = new DHPhieuDO();
            ToDO(o); return o;
        }
    }
}
