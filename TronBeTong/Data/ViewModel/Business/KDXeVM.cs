using System.Runtime.CompilerServices;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class KDXeVM: VMBase
    {
        public int Id { get; set; }

        private string? _bsx;
        public string? BSX { get { return _bsx; } set { if (_bsx != value) { _bsx = value; NotifyChanged(); } } }

        private double _dt;
        public double DungTich { get { return _dt; } set { if (_dt != value) { _dt = value; NotifyChanged(); } } }

        private double _dtmin;
        public double DungTichMin { get { return _dtmin; } set { if (_dtmin != value) { _dtmin = value; NotifyChanged(); } } }
    
        public bool IsChanged { get; set; }

        public KDXeVM() { }
        public KDXeVM(KDXeDO o) {
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
            BSX = null;
            DungTich = 0;
            DungTichMin = 0;
            IsChanged = false;
        }

        public void CopyFrom(KDXeVM o)
        {
            Id = o.Id;
            BSX = o.BSX;
            DungTich = o.DungTich;
            DungTichMin = o.DungTichMin;
        }

        public void FromDO(KDXeDO o)
        {
            Id = o.Id;
            BSX = o.BSX;
            DungTich = o.DungTich;
            DungTichMin = o.DungTichMin;
        }

        public void ToDO(KDXeDO o)
        {
            o.Id = Id;
            o.BSX = BSX;
            o.DungTich = DungTich;
            o.DungTichMin = DungTichMin;
        }

        public KDXeDO CreateDO()
        {
            KDXeDO o = new();
            ToDO(o);
            return o;
        }
    }
}
