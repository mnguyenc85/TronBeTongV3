using System.Threading.Tasks;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class TKPhieuDO
    {
        public int Id { get; set; }
        public int DonHangId { get; set; }

        public string? SoPhieu { get; set; }
        public int XeId { get; set; }
        public int LaiXeId { get; set; }
        public int CongThucId { get; set; }

        public int MeDat { get; set; }
        public int MeHT { get; set; }
        public double TheTichDat { get; set; }
        public double TheTichHT { get; set; }
        public double KLDat { get; set; }
        public double KLHT { get; set; }

        public DateTime? TGBD { get; set; }
        public DateTime? TGHT { get; set; }

        public double DonGia { get; set; }
        public int DonStt { get; set; }
        public double DonM3 { get; set; }

        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }

        public string? KepChi { get; set; }

        public THKhoiLuongTheoTP KLTheoTP { get; set; } = new THKhoiLuongTheoTP();

        public TKPhieuDO() { }
        public TKPhieuDO(DHPhieuDO o)
        {
            FromDHPhieuDO(o);
        }

        public void FromDHPhieuDO(DHPhieuDO o)
        {
            Id = o.Id;
            DonHangId = o.DonHangId;
            SoPhieu = o.SoPhieu;
            XeId = o.XeId;
            LaiXeId = o.LaiXeId;
            CongThucId = o.CongThucId;
            MeDat = o.MeDat;
            MeHT = o.MeHT;
            TheTichDat = o.TheTichDat;
            TheTichHT = o.TheTichHT;
            KLDat = o.KLDat;
            KLHT = o.KLHT;
            TGBD = o.TGBD;
            TGHT = o.TGHT;
            DonGia = o.DonGia;
            DonM3 = o.DonM3;
            DonStt = o.DonStt;

            TrangThai = o.TrangThai;
            GhiChu = o.GhiChu;

            KepChi = o.KepChi;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dct">Danh sách công thức</param>
        /// <param name="uniqueMaTP"></param>
        /// <param name="flags"></param>
        /// <param name="mophong">Load mẻ mô phỏng</param>
        /// <returns></returns>
        public async Task TongHop(Dictionary<int, TKCongThucDO> dct, List<TKThanhPhanDO> uniqueMaTP, int flags, bool mophong)
        {
            if (dct.TryGetValue(CongThucId, out TKCongThucDO? ct))
            {
                var lstme = await DbBridge.Instance.HT_Me_LoadByPhieu(Id, mophong);
                KLTheoTP.TongHop(ct, uniqueMaTP, lstme, flags);
            }
        }
    }
}
