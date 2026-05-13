using NMWPFControls.Core.MVVM;
using System.Runtime.CompilerServices;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class KDLaiXeVM: VMBase
    {
        public int Id { get; set; }

        private string? _ten;
        public string? Ten { get { return _ten; } set { if (_ten != value) { _ten = value; NotifyChanged(); } } }

        private string? _sdt;
        public string? Sdt { get { return _sdt; } set { if (_sdt != value) { _sdt = value; NotifyChanged(); } } }

        public bool IsChanged { get; set; }

        public KDLaiXeVM() { }
        public KDLaiXeVM(KDLaiXeDO o)
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
            Ten = null;
            Sdt = null;
            IsChanged = false;
        }

        public void CopyFrom(KDLaiXeVM o)
        {
            Id = o.Id;
            Ten = o.Ten;
            Sdt = o.Sdt;
        }

        public void FromDO(KDLaiXeDO o)
        {
            Id = o.Id;
            Ten = o.Ten;
            Sdt = o.Sdt;
        }

        public void ToDO(KDLaiXeDO o)
        {
            o.Id = Id;
            o.Ten = Ten;
            o.Sdt = Sdt;
        }

        public KDLaiXeDO CreateDO()
        {
            KDLaiXeDO o = new();
            ToDO(o);
            return o;
        }
    }
}
