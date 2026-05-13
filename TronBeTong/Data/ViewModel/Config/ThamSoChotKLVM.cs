using NMWPFControls.Core.MVVM;

namespace TronBeTongV3.Data.ViewModel.Config
{
    public class ThamSoChotKLVM: VMBase
    {
        private string? _tencan, _tenbien;
        public string? TenCan { get { return _tencan; } set { if (_tencan != value) { _tencan = value; NotifyChanged(); } } }
        public string? TenBien { get { return _tenbien; } set { if (_tenbien != value) { _tenbien = value; NotifyChanged(); } } }

        private string? _giatri;
        public string? GiaTri { get { return _giatri; } set { if (_giatri != value) { _giatri = value; NotifyChanged(); } } }
    }
}
