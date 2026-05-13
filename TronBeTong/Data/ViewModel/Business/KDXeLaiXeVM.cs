using NMWPFControls.Core.MVVM;

namespace TronBeTongV3.Data.ViewModel
{
    public class KDXeLaiXeVM: VMBase
    {
        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        public int XeId { get; set; }
        public int LaiXeId { get; set; }
        public KDXeVM? Xe { get; set; }
        public KDLaiXeVM? LaiXe { get; set; }
    }
}
