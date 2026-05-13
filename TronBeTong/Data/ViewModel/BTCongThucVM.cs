using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Core.MVVM;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.ViewModel
{
    public class BTCongThucVM: VMBase
    {
        public int Id { get; set; }

        private int _stt;
        public int STT { get { return _stt; } set { if (_stt != value) { _stt = value; NotifyChanged(); } } }

        private string? _ma;
        public string? Ma { get { return _ma; } set { if (_ma != value) { _ma = value; NotifyChanged(); } } }

        private string? _mac;
        public string? Mac { get { return _mac; } set { if (_mac != value) { _mac = value; NotifyChanged(); } } }

        private string? _slump;
        public string? Slump { get { return _slump; } set { if (_slump != value) { _slump = value; NotifyChanged(); } } }

        private double _wcratio;
        public double WCRatio { get { return _wcratio; } set { if (_wcratio != value) { _wcratio = value; NotifyChanged(); } } }

        private double _kthat;
        public double KTHat { get { return _kthat; } set { if (_kthat != value) { _kthat = value; NotifyChanged(); } } }

        private double _klnuoc;
        public double KLNuoc { get { return _klnuoc; } set { if (_klnuoc != value) { _klnuoc = value; NotifyChanged(); } } }

        private int _sotp;
        public int SoTP { get { return _sotp; } set { if (_sotp != value) { _sotp = value; NotifyChanged(); } } }

        public ObservableCollection<BTThanhPhanVM> DsThanhPhan { get; private set; } = [];

        public bool IsChanged { get; set; }

        protected override void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            base.NotifyChanged(propertyName);
            IsChanged = true;
        }

        private DHKgSilos? _klsilos;
        public DHKgSilos? KLSilos { get { return _klsilos; } set { if (_klsilos != value) { _klsilos = value; NotifyChanged(); } } }

        public BTCongThucVM() { }
        public BTCongThucVM(BTCongThucDO o)
        {
            FromDO(o);
        }

        public void CalWCRatio()
        {
            double tongklxi = 0;
            double tongklnuoc = 0;

            foreach (var tp in DsThanhPhan)
            {
                if (tp.NL?.PhanLoai == Core.LoaiThanhPhan.XiMang) tongklxi += tp.KL;
                else if (tp.NL?.PhanLoai == Core.LoaiThanhPhan.Nuoc) tongklnuoc += tp.KL;
            }

            KLNuoc = tongklnuoc;
            if (tongklxi > 0) 
                WCRatio = Math.Round(KLNuoc / tongklxi, 3);
        }

        public double CalTotalWeight()
        {
            double tongkl = 0;
            foreach (var tp in DsThanhPhan)
            {
                tongkl += tp.KL;
            }
            return tongkl;
        }

        public void AddThanhPhan(SiloNguyenLieuVM nl)
        {
            BTThanhPhanVM tp = new BTThanhPhanVM()
            {
                STT = DsThanhPhan.Count + 1,
                Id = -1,
                NL = nl,
                State = Core.ViewModelStates.Add
            };
            DsThanhPhan.Add(tp);
        }

        public void RemoveThanhPhan(BTThanhPhanVM tp)
        {
            if (tp.Id > 0)
            {
                tp.State = Core.ViewModelStates.Remove;
            }            
            else
            {
                DsThanhPhan.Remove(tp);
            }
        }

        public void Reset()
        {
            Id = -1;
            Ma = null;
            Mac = null;
            Slump = null;
            KLNuoc = 0;
            WCRatio = 0;
            KTHat = 0;
            SoTP = 0;
            DsThanhPhan.Clear();
            IsChanged = false;
        }

        public BTCongThucVM Clone()
        {
            return new BTCongThucVM()
            {
                Id = Id,
                Ma = Ma,
                Mac = Mac,
                Slump = Slump,
                KLNuoc = KLNuoc,
                WCRatio = WCRatio,
                KTHat = KTHat,
                SoTP = SoTP,
            };
        }

        public void CopyFrom(BTCongThucVM o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Mac = o.Mac;
            Slump = o.Slump;
            KLNuoc = o.KLNuoc;
            WCRatio = o.WCRatio;
            KTHat = o.KTHat;
            SoTP = o.SoTP;
        }

        public void FromDO(BTCongThucDO o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Mac = o.Mac;
            Slump = o.Slump;
            KLNuoc = o.KLNuoc;
            WCRatio = o.WCRatio;
            KTHat = o.KTHat;
            SoTP = o.SoTP;
        }

        public void ToDO(BTCongThucDO o)
        {
            o.Id = Id;
            o.Ma = Ma;
            o.Mac = Mac;
            o.Slump = Slump;
            o.KLNuoc = KLNuoc;
            o.WCRatio = WCRatio;
            o.KTHat = KTHat;
            o.SoTP = SoTP;
        }

        public BTCongThucDO CreateDO()
        {
            BTCongThucDO o = new();
            ToDO(o);
            return o;
        }
    }
}
