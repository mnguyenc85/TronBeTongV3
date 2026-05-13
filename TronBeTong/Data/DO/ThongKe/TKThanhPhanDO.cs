using TronBeTongV3.Core;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class TKThanhPhanDO
    {
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public int SiloNo { get; set; }
        public LoaiThanhPhan PhanLoai { get; set; }

        public int RoundDigit { get; set; }

        public TKThanhPhanDO() { }
        public TKThanhPhanDO(string ma, string? ten, LoaiThanhPhan pl, int silo, int rdigit) {
            Ma = ma;
            Ten = ten;
            PhanLoai = pl;
            SiloNo = silo;
            RoundDigit = rdigit;
        }
    }
}
