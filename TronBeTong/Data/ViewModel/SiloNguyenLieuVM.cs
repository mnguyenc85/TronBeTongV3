using System;
using System.Runtime.CompilerServices;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Core;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class SiloNguyenLieuVM: VMBase
    {
        public int Id { get; set; }

        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        private string? _ma;
        public string? Ma { get { return _ma; } set { if (_ma != value) { _ma = value; NotifyChanged(); } } }

        private string? _ten;
        public string? Ten { get { return _ten; } set { if (_ten != value) { _ten = value; NotifyChanged(); } } }

        private LoaiThanhPhan _pl;
        public LoaiThanhPhan PhanLoai { get { return _pl; } set { if (_pl != value) { _pl = value; NotifyChanged(); } } }

        private double _da;
        public double DoAm { get { return _da; } set { if (_da != value) { _da = value; NotifyChanged(); } } }

        public bool IsChanged { get; set; }

        public SiloNguyenLieuVM() { }
        public SiloNguyenLieuVM(SiloNguyenLieuDO o)
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
            PhanLoai = LoaiThanhPhan.None;
            DoAm = 0;
            IsChanged = false;
        }

        public void CopyFrom(SiloNguyenLieuVM o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Ten = o.Ten;  
            PhanLoai = o.PhanLoai;
            DoAm = o.DoAm;
        }

        public void FromDO(SiloNguyenLieuDO o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Ten = o.Ten;
            PhanLoai = (LoaiThanhPhan)o.PhanLoai;
            DoAm = o.DoAm;
        }

        public void ToDO(SiloNguyenLieuDO o)
        {
            o.Id = Id;
            o.Ma = Ma;
            o.Ten = Ten;
            o.PhanLoai = (int)PhanLoai;
            o.DoAm = DoAm;
        }

        public SiloNguyenLieuDO CreateDO()
        {
            SiloNguyenLieuDO o = new();
            ToDO(o);
            return o;
        }
    }
}
