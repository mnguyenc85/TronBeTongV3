using System;

namespace TronBeTongV3.Core
{
    public enum LoaiThanhPhan { None = 0, CotLieu = 1, XiMang = 2, PhuGia = 3, Nuoc = 4 }

    public enum ButtonTypes { 
        None = 0, VanDauVao = 1, VanDauRa = 2, DongCo = 3, DamRung = 4, ChuyenTuDong = 5, 
        CmnConfig = 6, 
        GuiCapPhoi = 7, GuiM3 = 8, GuiSoMe = 9,
        DoAm = 10, ThemBot = 11,
        RuaCoi = 12
    }

    public enum ViewModelStates { None = 0, Add = 1, Update = 2, Remove = 3 }

    public enum TrangThaiCanCL
    {
        Rong = 0,
        ThoCL1 = 1, ChoCL1 = 2,
        ThoCL2 = 3, ChoCL2 = 4,
        ThoCL3 = 5, ChoCL3 = 6,
        ThoCL4 = 7, ChoCL4 = 8,
        Du = 9, Day = 10
    }

    public enum TrangThaiCanXi
    {
        Rong = 0,
        ThoXi1 = 1, TinhXi1 = 2, ChoXi1 = 3,
        Du = 4, Day = 5
    }

    public enum TrangThaiCanNuoc
    {
        Rong = 0,
        ThoNuoc = 1, TinhNuoc = 2, ChoNuoc = 3,
        Du = 4, Day = 5
    }
}
