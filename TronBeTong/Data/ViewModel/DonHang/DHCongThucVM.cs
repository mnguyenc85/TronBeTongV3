using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel.DonHang
{
    public class DHCongThucVM: VMBase
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
        /// <summary>
        /// Kích thước hạt (AggMax)
        /// </summary>
        public double KTHat { get { return _kthat; } set { if (_kthat != value) { _kthat = value; NotifyChanged(); } } }

        private double _klnuoc;
        public double KLNuoc { get { return _klnuoc; } set { if (_klnuoc != value) { _klnuoc = value; NotifyChanged(); } } }

        private int _sotp;
        public int SoTP { get { return _sotp; } set { if (_sotp != value) { _sotp = value; NotifyChanged(); } } }

        public ObservableCollection<DHThanhPhanVM> DsThanhPhan { get; private set; } = [];

        public bool IsChanged { get; set; }

        protected override void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            base.NotifyChanged(propertyName);
            IsChanged = true;
        }

        public DHCongThucVM() { }
        public DHCongThucVM(BTCongThucDO o)
        {
            FromDO(o);
        }

        public void AddThanhPhan(SiloNguyenLieuVM nl)
        {
            DHThanhPhanVM tp = new()
            {
                STT = DsThanhPhan.Count + 1,
                Id = -1,
                NL_Ma = nl.Ma,
                NL_Ten = nl.Ten,
                NL_PhanLoai = nl.PhanLoai,
            };
            DsThanhPhan.Add(tp);
        }

        public void ApplyM3(double m3)
        {
            foreach (var tp in DsThanhPhan)
            {
                tp.KLTong = tp.KLCongThuc * m3;
            };
        }

        public void ApplySoMe(int me)
        {
            foreach (var tp in DsThanhPhan)
            {
                if (me > 0)
                    tp.KLMe = Math.Round(tp.KLTong / me, 3);
                else
                    tp.KLMe = 0;
            }
        }

        #region VM <-> DO
        public DHCongThucVM Clone()
        {
            return new DHCongThucVM()
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

        public void CopyFrom(DHCongThucVM o)
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
        #endregion
    }
}
