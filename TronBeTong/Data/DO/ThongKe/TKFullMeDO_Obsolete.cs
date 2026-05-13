namespace TronBeTongV3.Data.DO.ThongKe
{
    [Obsolete]
    public class TKFullMeDO_Obsolete
    {
        public int DonId { get; set; }
        public int PhieuId { get; set; }
        public int MeId { get; set; }

        public string? DonMa { get; set; }
        public string? DonKhachHang { get; set; }
        public string? DonDuAn { get; set; }
        public string? PhieuMa { get; set; }
        public string? PhieuBSX { get; set; }
        public string? PhieuLaiXe { get; set; }
        public string? PhieuCapPhoi { get; set; }
        public string? TGHT { get; set; }

        public string? M3Tron { get; set; }
        public string? SoMe { get; set; }

        public string[] TPs { get; set; }
        public double[] KLs { get; set; }


        public string? TrangThai { get; set; }
        public int Flags { get; set; }

        public TKFullMeDO_Obsolete(int sotp)
        {
            TPs = new string[sotp];
            KLs = new double[sotp];
        }
        public TKFullMeDO_Obsolete Clone_NoTPs()
        {
            TKFullMeDO_Obsolete n = new(TPs.Length);

            n.DonId = DonId;
            n.PhieuId = PhieuId;
            n.MeId = MeId;

            n.DonMa = DonMa;
            n.DonKhachHang = DonKhachHang;
            n.DonDuAn = DonDuAn;
            n.PhieuMa = PhieuMa;
            n.PhieuBSX = PhieuBSX;
            n.PhieuLaiXe = PhieuLaiXe;
            n.PhieuCapPhoi = PhieuCapPhoi;
            n.TGHT = TGHT;
            n.M3Tron = M3Tron;
            n.SoMe = SoMe;
            n.TrangThai = TrangThai;
            n.Flags = Flags;

            return n;
        }

        public void TPs4KLs(List<TKThanhPhanDO> uniqueMas)
        {
            for (int i = 0; i < TPs.Length; i++)
            {
                TPs[i] = Math.Round(KLs[i], uniqueMas[i].RoundDigit).ToString();
            }
        }
    }
}
