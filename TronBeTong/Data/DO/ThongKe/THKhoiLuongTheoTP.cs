using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class THKhoiLuongTheoTP
    {
        public TKCongThucDO? CongThuc { get; set; }

        public List<TKThanhPhanDO> uniqueMaTP { get; private set; } = [];
        public string?[]? DsCTTen { get; set; }
        public double[]? CTKLs { get; set; }
        public List<TKMeDO> DsMe { get; private set; } = [];

        public double[]? TongKL { get; set; }

        public void TongHop(TKCongThucDO ct, List<TKThanhPhanDO> dstp, List<DHMeDO> dsme, int flags)
        {
            CongThuc = ct;
            uniqueMaTP = dstp;
            DsCTTen = new string[uniqueMaTP.Count];
            CTKLs = new double[uniqueMaTP.Count];
            TongKL = new double[uniqueMaTP.Count];

            for (int i = 0; i < uniqueMaTP.Count; i++)
            {
                var tp = uniqueMaTP[i];
                foreach (var cttp in ct.DsThanhPhan)
                {
                    if (cttp.NL_Ma == tp.Ma)
                    {
                        DsCTTen[i] = cttp.NL_Ten;
                        CTKLs[i] += cttp.KLCongThuc;
                    }
                }
            }

            foreach (var me in dsme)
            {
                if ((me.Flags & flags) == 0) 
                    continue;

                TKMeDO me1 = new(uniqueMaTP.Count)
                {
                    Id = me.Id,
                    STT = me.STT,
                    M3Tron = me.M3Tron,
                    TGHT = me.CreateAt,
                    Flags = me.Flags,
                };
                for (int i = 0; i < uniqueMaTP.Count; i++)
                {
                    var tp = uniqueMaTP[i];
                    foreach (var cttp in ct.DsThanhPhan)
                    {
                        if (cttp.NL_Ma == tp.Ma)
                        {
                            if (cttp.NL_PhanLoai == (int)Core.LoaiThanhPhan.CotLieu)
                            {
                                me1.KLs[i] += me.KLCL[cttp.NL_Silo];
                            }
                            else if (cttp.NL_PhanLoai == (int)Core.LoaiThanhPhan.XiMang)
                            {
                                me1.KLs[i] += me.KLXi[cttp.NL_Silo];
                            }
                            else if (cttp.NL_PhanLoai == (int)Core.LoaiThanhPhan.PhuGia)
                            {
                                me1.KLs[i] += me.KLPG[cttp.NL_Silo];
                            }
                            else if (cttp.NL_PhanLoai == (int)Core.LoaiThanhPhan.Nuoc)
                            {
                                me1.KLs[i] += me.KLNuoc;
                            }
                            TongKL[i] += me1.KLs[i];
                        }
                    }
                }
                DsMe.Add(me1);
            }
        }
    }
}
