using System.Runtime.CompilerServices;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class KDKhachHangVM: VMBase
    {
        public int Id { get; set; }
        public int STT { get; set; }

        private string? _ma;
        public string? Ma { get { return _ma; } set { if (_ma != value) { _ma = value; NotifyChanged(); } } }

        private string? _ten;
        public string? Ten { get { return _ten; } set { if (_ten != value) { _ten = value; NotifyChanged(); } } }

        private string? _diachi;
        public string? DiaChi { get { return _diachi; } set { if (_diachi != value) { _diachi = value; NotifyChanged(); } } }

        private string? _sdt;
        public string? Sdt { get { return _sdt; } set { if (_sdt != value) { _sdt = value; NotifyChanged(); } } }

        private string? _email;
        public string? Email { get { return _email; } set { if (_email != value) { _email = value; NotifyChanged(); } } }

        private string? _mst;
        public string? MaSoThue { get { return _mst; } set { if (_mst != value) { _mst = value; NotifyChanged(); } } }

        private string? _lienhe;
        public string? LienHe { get { return _lienhe; } set { if (_lienhe != value) { _lienhe = value; NotifyChanged(); } } }

        private string? _ghichu;
        public string? GhiChu { get { return _ghichu; } set { if (_ghichu != value) { _ghichu = value; NotifyChanged(); } } }

        //private bool _isChanged;
        //public bool IsChanged { get { return _isChanged; } set { if (_isChanged != value) { _isChanged = value; NotifyChanged(); } } }
        public bool IsChanged { get; set; }
        
        public DateTime Updated { get; set; }

        public KDKhachHangVM() { }
        public KDKhachHangVM(KDKhachHangDO o)
        {
            FromDO(o);
        }

        protected override void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            base.NotifyChanged(propertyName);
            IsChanged = true;
        }

        public void Reset()
        {
            Id = -1;
            Ma = null;
            Ten = null;
            DiaChi = null;
            Sdt = null;
            Email = null;
            MaSoThue = null;
            LienHe = null;
            GhiChu = null;
            Updated = DateTime.MinValue;

            IsChanged = false;            
        }

        public void CopyFrom(KDKhachHangVM o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Ten = o.Ten;
            DiaChi = o.DiaChi;
            Sdt = o.Sdt;
            Email = o.Email;
            MaSoThue = o.MaSoThue;
            LienHe = o.LienHe;
            GhiChu = o.GhiChu;
            Updated = o.Updated;
        }

        public void FromDO(KDKhachHangDO o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Ten = o.Ten;
            DiaChi = o.DiaChi;
            Sdt = o.Sdt;
            Email = o.Email;
            MaSoThue = o.MaSoThue;
            LienHe = o.LienHe;
            GhiChu = o.GhiChu;
            Updated = o.UpdatedAt;
        }

        public void ToDO(KDKhachHangDO o)
        {
            o.Id = Id;
            o.Ma = Ma;
            o.Ten = Ten;
            o.DiaChi = DiaChi;
            o.Sdt = Sdt;
            o.Email = Email;
            o.MaSoThue = MaSoThue;
            o.LienHe = LienHe;
            o.GhiChu = GhiChu;
            o.UpdatedAt = Updated;
        }

        public KDKhachHangDO CreateDO()
        {
            KDKhachHangDO o = new();
            ToDO(o);
            return o;
        }
    }
}
