using NMWPFControls.Core.MVVM;
using System.Runtime.CompilerServices;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class KDDuAnVM: VMBase
    {
        public int Id { get; set; }
        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        private string? _da;
        public string? DuAn { get { return _da; } set { if (_da != value) { _da = value; NotifyChanged(); } } }

        private string? _ct;
        public string? CongTrinh { get { return _ct; } set { if (_ct != value) { _ct = value; NotifyChanged(); } } }

        private string? _hm;
        public string? HangMuc { get { return _hm; } set { if (_hm != value) { _hm = value; NotifyChanged(); } } }

        private string? _dc;
        public string? DiaChi { get { return _dc; } set { if (_dc != value) { _dc = value; NotifyChanged(); } } }

        private string? _gc;
        public string? GhiChu { get { return _gc; } set { if (_gc != value) { _gc = value; NotifyChanged(); } } }

        private int _khid;
        public int KHId { get { return _khid; } set { if (_khid != value) { _khid = value; NotifyChanged(); } } }


        private string? _khma;
        public string? KHMa { get { return _khma; } set { if (_khma != value) { _khma = value; NotifyChanged(); } } }


        private bool _isChanged;
        public bool IsChanged { get { return _isChanged; } set { if (_isChanged != value) { _isChanged = value; NotifyChanged(); } } }

        public KDDuAnVM() { }

        protected override void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != "IsChanged")
            {
                base.NotifyChanged(propertyName);
                IsChanged = true;
            }
        }

        public void Reset()
        {
            Id = -1;
            DuAn = null;
            CongTrinh = null;
            HangMuc = null;
            DiaChi = null;
            GhiChu = null;
            KHId = -1;
            IsChanged = false;

            STT = 0;
        }

        public void CopyFrom(KDDuAnVM o)
        {
            Id = o.Id;
            DuAn = o.DuAn;
            CongTrinh = o.CongTrinh;
            HangMuc = o.HangMuc;
            DiaChi = o.DiaChi;
            GhiChu = o.GhiChu;
            KHId = o.KHId;
            KHMa = o.KHMa;

            STT = o.STT;
        }

        public void ToDO(KDDuAnDO o)
        {
            o.Id = Id;
            o.DuAn = DuAn;
            o.CongTrinh = CongTrinh;
            o.HangMuc = HangMuc;
            o.DiaChi = DiaChi;
            o.GhiChu = GhiChu;
            o.KHId = KHId;
        }

        public KDDuAnDO CreateDO()
        {
            var o = new KDDuAnDO();
            ToDO(o);
            return o;
        }
    }
}
