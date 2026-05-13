namespace TronBeTongV3.Reports
{
    public class HackValue
    {
        public string? SoPhieu { get; set; }
        public string? NgayTron { get; set; }

        public string? KhachHang { get; set; }
        public string? DuAn { get; set; }
        public string? DuAnDiaChi { get; set; }

        public string? BienSoXe { get; set; }
        public string? LaiXe { get; set; }

        public string? BeTongMac { get; set; }
        public string? BeTongSlump { get; set; }
        public string? BeTongMaxAgg { get; set; }

        public string? MaSo { get; set; }
        public string? TGRoiTram { get; set; }


        public string? M3 { get; set; }
        public string? TichLuy { get; set; }

        public void Reset()
        {
            SoPhieu = null;
            NgayTron = null;
            KhachHang = null;
            DuAn = null;
            DuAnDiaChi = null;
            BienSoXe = null;
            LaiXe = null;
            BeTongMac = null;
            BeTongSlump = null;
            BeTongMaxAgg = null;
            MaSo = null;
            TGRoiTram = null;
            M3 = null;
            TichLuy = null;
        }
    }
}
