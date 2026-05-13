namespace TronBeTongV3.Data.DO.DonHang
{
    /// <summary>
    /// Hiển thị cấp phối theo silo
    /// </summary>
    public class DHKgSilos
    {
        public double[] CLs { get; set; }
        public double[] Xis { get; set; }
        public double[] PGs { get; set; }
        public double Nuoc { get; set; }

        public string? TenKhac { get; set; }
        public double KLKhac { get; set; }

        public DHKgSilos(int socl, int soxi, int sopg) { 
            CLs = new double[socl];
            Xis = new double[soxi];
            PGs = new double[sopg];
        }
    }
}
