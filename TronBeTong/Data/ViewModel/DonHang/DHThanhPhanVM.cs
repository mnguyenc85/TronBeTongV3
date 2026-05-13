using NMWPFControls.Core.MVVM;
using TronBeTongV3.Core;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.ViewModel
{
    public class DHThanhPhanVM: VMBase, IComparable<DHThanhPhanVM>
    {
        public int Id { get; set; }

        public LoaiThanhPhan NL_PhanLoai { get; set; }
        public int NL_SiloIndex { get; set; }
        public string? NL_Ma { get; set; }
        public string? NL_Ten { get; set; }

        private double _klct;
        /// <summary>
        /// Khối lượng thành phần theo công thức
        /// </summary>
        public double KLCongThuc { get { return _klct; } set { if (_klct != value) { _klct = value; NotifyChanged(); } } }
        private double _kltong;
        /// <summary>
        /// Khối lượng tổng
        /// </summary>
        public double KLTong { get { return _kltong; } set { if (_kltong != value) { _kltong = value; NotifyChanged(); } } }
        private double _klme;
        /// <summary>
        /// Khối lượng mỗi mẻ
        /// </summary>
        public double KLMe { get { return _klme; } set { if (_klme != value) { _klme = value; NotifyChanged(); } } }

        private string? _siloTen;
        public string? SiloTen { get { return _siloTen; } set { if (_siloTen != value) { _siloTen = value; NotifyChanged(); } } }

        private int _stt;
        /// <summary>
        /// Số thứ tự (View only)
        /// </summary>
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        public DHThanhPhanVM() { }
        public DHThanhPhanVM(DHThanhPhanDO o) { FromDO(o); }

        public void FromDO(DHThanhPhanDO o)
        {
            Id = o.Id;
            NL_Ma = o.NL_Ma;
            NL_Ten = o.NL_Ten;
            NL_PhanLoai = (LoaiThanhPhan)o.NL_PhanLoai;
            NL_SiloIndex = o.NL_Silo;
            KLCongThuc = o.KLCongThuc;
            KLTong = o.KLTong;
            KLMe = o.KLMe;
        }

        public void ToDO(DHThanhPhanDO o)
        {
            o.Id = Id;
            o.NL_Ma = NL_Ma;
            o.NL_Ten = NL_Ten;
            o.NL_PhanLoai = (int)NL_PhanLoai;
            o.NL_Silo = NL_SiloIndex;
            o.KLCongThuc = Math.Round(KLCongThuc, 3);
            o.KLTong = Math.Round(KLTong, 3);
            o.KLMe = Math.Round(KLMe, 3);
        }

        public DHThanhPhanDO CreateDO()
        {
            var o = new DHThanhPhanDO();
            ToDO(o);
            return o;
        }

        public int CompareTo(DHThanhPhanVM? other)
        {
            if (other == null) return 1;
            if (other.NL_PhanLoai == NL_PhanLoai) return NL_SiloIndex.CompareTo(other.NL_SiloIndex);
            return NL_PhanLoai.CompareTo(other.NL_PhanLoai);
        }
    }
}
