using NMWPFControls.Core.MVVM;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.ViewModel.DonHang
{
    public class DHDonVM: VMBase
    {
        public int Id { get; set; }

        private string? _ma;
        public string? Ma { get { return _ma; } set { if (_ma != value) { _ma = value; NotifyChanged(); } } }

        private KDKhachHangVM? _kh;
        public KDKhachHangVM? KhachHang { get { return _kh; } set { if (_kh != value) { _kh = value; NotifyChanged(); } } }

        private KDDuAnVM? _da;
        public KDDuAnVM? DuAn { get { return _da; } set { if (_da != value) { _da = value; NotifyChanged(); } } }

        private double _ttdh;
        public double TheTichDH { get { return _ttdh; } set { if (_ttdh != value) { _ttdh = value; NotifyChanged(); } } }

        private double _thht;
        public double TheTichHT { get { return _thht; } set { if (_thht != value) { _thht = value; NotifyChanged(); } } }
        private double _klht;
        public double KLHT { get { return _klht; } set { if (_klht != value) { _klht = value; NotifyChanged(); } } }
        private int _meht;
        public int MeHT { get { return _meht; } set { if (_meht != value) { _meht = value; NotifyChanged(); } } }
        private DateTime? _tght;
        public DateTime? TGHT { get { return _tght; } set { if (_tght != value) { _tght = value; NotifyChanged(); } } }

        public string? GhiChu { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        private int _tsp;
        public int TongSoPhieu { get { return _tsp; } set { if (_tsp != value) { _tsp = value; NotifyChanged(); } } }

        private int _tt;
        public int TrangThai { get { return _tt; } set { if (_tt != value) { _tt = value; NotifyChanged(); } } }

        [XmlIgnore]
        public int ActiveLv { get; set; }

        [XmlIgnore]
        public ObservableCollection<DHPhieuVM> DsPhieu { get; private set; } = [];

        public DHDonVM() { }
        public DHDonVM(DHDonDO o)
        {
            FromDO(o);
        }

        public void AddPhieu(DHPhieuVM ph)
        {
            DsPhieu.Add(ph);
            ph.DonHang = this;
        }

        public void Reset()
        {
            Id = -1;
            Ma = null;
            GhiChu = null;

            KhachHang?.Reset();
            DuAn?.Reset();

            TheTichDH = 0;

            TheTichHT = 0;
            KLHT = 0;
            MeHT = 0;
            TGHT = null;
            CreatedAt = DateTime.Now;            
        }

        public void CopyFrom(DHDonVM d)
        {
            Id = d.Id;
            Ma = d.Ma;
            GhiChu = d.GhiChu;

            if (d.KhachHang != null)
            {
                KhachHang ??= new KDKhachHangVM();
                KhachHang.CopyFrom(d.KhachHang);
            }
            else
            {
                KhachHang?.Reset();
            }

            if (d.DuAn != null)
            {
                DuAn ??= new KDDuAnVM();
                DuAn.CopyFrom(d.DuAn);
            }
            else
            {
                DuAn?.Reset();
            }

            TheTichDH = d.TheTichDH;

            TheTichHT = d.TheTichHT;
            KLHT = d.KLHT;
            MeHT = d.MeHT;
            TGHT = d.TGHT;

            CreatedAt = d.CreatedAt;

            TongSoPhieu = d.TongSoPhieu;
        }

        public DHDonVM Clone()
        {
            return new DHDonVM()
            {
                Id = Id,
                Ma = Ma,
                GhiChu = GhiChu,

                KhachHang = KhachHang,
                DuAn = DuAn,
                //DuAn = new KDDuAnVM()
                //{
                //    Id = -1,
                //    DuAn = DuAn?.DuAn,
                //    CongTrinh = DuAn?.CongTrinh,
                //    HangMuc = DuAn?.HangMuc,
                //    DiaChi = DuAn?.DiaChi,
                //},

                TheTichDH = TheTichDH,

                TheTichHT = TheTichHT,
                KLHT = KLHT,
                MeHT = MeHT,
                TGHT = TGHT,

                CreatedAt = CreatedAt,

                TongSoPhieu = TongSoPhieu,
            };
        }

        public void FromDO(DHDonDO o)
        {
            Id = o.Id;
            Ma = o.Ma;

            // TODO: Lấy DuAn, CongTrinh, HangMuc, DiaChi từ DO?

            TheTichDH = o.TheTichDH;

            TheTichHT = Math.Round(o.TheTichHT, 1);
            KLHT = Math.Round(o.KLHT, 1);
            MeHT = o.MeHT;  
            TGHT = o.TGHT;

            GhiChu = o.GhiChu;
            CreatedAt = o.CreatedAt;

            TongSoPhieu = o.TongSoPhieu;
        }

        public void ToDO(DHDonDO o)
        {
            o.Id = Id;
            o.Ma = Ma;

            o.KhachHangId = _kh != null? _kh.Id: -1;

            if (_da != null)
            {
                o.DuAnId = _da.Id;
                o.DuAn = _da.DuAn;
                o.CongTrinh = _da.CongTrinh;
                o.HangMuc = _da.HangMuc;
                o.DiaChi = _da.DiaChi;
            }
            else o.DuAnId = -1;

            o.TheTichDH = TheTichDH;

            o.TheTichHT = TheTichHT;
            o.KLHT = KLHT;
            o.MeHT = MeHT;
            o.TGHT = TGHT;

            o.GhiChu = GhiChu;
            o.CreatedAt = CreatedAt;

            //o.TongSoPhieu = TongSoPhieu;
        }

        public DHDonDO CreateDO()
        {
            var o = new DHDonDO();
            ToDO(o);
            return o;
        }
    }
}
