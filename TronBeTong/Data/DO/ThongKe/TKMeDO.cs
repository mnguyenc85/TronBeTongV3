namespace TronBeTongV3.Data.DO.ThongKe
{
    public class TKMeDO
    {
        public int Id { get; set; }

        public int STT { get; set; }

        public double M3Tron { get; set; }
        public DateTime? TGHT { get; set; }

        public int Flags { get; set; }
        public double[] KLs { get; set; }

        public double[] DoAms { get; set; }

        public TKMeDO(int sotp) { 
            KLs = new double[sotp];
            DoAms = new double[sotp];
        }
    }
}
