using CommunityToolkit.Mvvm.ComponentModel;

namespace ManualPrintDocket.ViewModel
{
    public partial class PhieuTronBeTongVM: ObservableObject
    {
        [ObservableProperty]
        private string? soPhieu;

        [ObservableProperty]
        private string? khachHangTen;

        [ObservableProperty]
        private string? duAnTen;
        [ObservableProperty]
        private string? duAnDiaChi;

        [ObservableProperty]
        private string? xeBienSo;
        [ObservableProperty]
        private string? xeLaiXe;

        [ObservableProperty]
        private string? beTongMac;
        [ObservableProperty]
        private string? beTongDoSut;
        [ObservableProperty]
        private string? beTongDkCotLieuMax;
        [ObservableProperty]
        private string? beTongPhuGia;

        [ObservableProperty]
        private string? _M3Tron;
        [ObservableProperty]
        private string? _TongM3Tron;

        [ObservableProperty]
        private DateTime _TGHT;
    }
}
