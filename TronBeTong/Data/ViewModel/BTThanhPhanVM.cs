using System;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Core;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{
    public class BTThanhPhanVM: VMBase
    {
        public int Id { get; set; }
        public SiloNguyenLieuVM? NL { get; set; }
        private double _kl;
        public double KL { get { return _kl; } set { if (_kl != value) { _kl = value; NotifyChanged(); } } }

        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        public LoaiThanhPhan LoaiSilo { get; set; }
        public int LoaiSiloIndex { get; set; }
        private string? _siloTen;
        public string? SiloTen { get { return _siloTen; } set { if (_siloTen != value) { _siloTen = value; NotifyChanged(); } } }

        private ViewModelStates _state = ViewModelStates.None;
        public ViewModelStates State { get { return _state; } set { if (_state != value) { _state = value; NotifyChanged(); } } }

        public BTThanhPhanVM Clone()
        {
            var tp1 = new BTThanhPhanVM();
            tp1.STT = _stt;
            tp1.NL = NL;
            tp1.KL = KL;
            return tp1;
        }

        public void FromDO(BTThanhPhanDO o)
        {
            Id = o.Id;
            KL = o.KL;
        }

        public void ToDO(BTThanhPhanDO o, int ctid)
        {
            o.Id = Id;
            o.NLId = NL != null ? NL.Id : -1;
            o.CTId = ctid;
            o.KL = KL;
        }

        public BTThanhPhanDO CreateDO(int ctid)
        {
            var o = new BTThanhPhanDO();
            ToDO(o, ctid);
            return o;
        }
    }
}
