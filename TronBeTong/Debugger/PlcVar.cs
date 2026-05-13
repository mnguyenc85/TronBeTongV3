using System;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Comm;

namespace TronBeTongV3.Debugger
{

    public class PlcVar : VMBase
    {
        public int STT { get; set; }
        public string? Ten { get; set; }
        public string? DiaChi { get; set; }

        private double _gt;
        public double GiaTri { get { return _gt; } set { if (_gt != value) { _gt = value; NotifyChanged(); } } }
        private double _updated;
        /// <summary>
        /// Thời điểm cập nhật dữ liệu
        /// </summary>
        public double TGCapNhat { get { return _updated; } set { if (_updated != value) { _updated = value; NotifyChanged(); } } }

        public bool TheoDoi { get; set; }

        private ModelTag _tag;

        public PlcVar(ModelTag tag) { _tag = tag; }

        public void UpdateFromTag(double t0)
        {
            GiaTri = _tag.Value;
            TGCapNhat = _tag.LastUpdated / 10000000d - t0;
        }
    }
}
