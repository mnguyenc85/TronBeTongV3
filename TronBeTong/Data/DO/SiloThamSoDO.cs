namespace TronBeTongV3.Data.DO
{
    public class SiloThamSoDO
    {
        public int Id { get; set; }
        public string? Ma { get; set; }

        public double KLXaCham { get; set; }
        public double ChuKyNhay { get; set; }
        public double TGDongNhay { get; set; }

        /// <summary>
        /// CoRung: 1; TuDongRung: 2
        /// </summary>
        public int Flags { get; set; }
    }
}
