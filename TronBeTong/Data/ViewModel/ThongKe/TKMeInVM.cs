using System;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.Data.ViewModel.ThongKe
{
    public class TKMeInVM
    {
        public string? STT { get; set; }
        public string? M3Tron { get; set; }

        // KLCL (0..5)
        public string? KLCL0 { get; set; }
        public string? KLCL1 { get; set; }
        public string? KLCL2 { get; set; }
        public string? KLCL3 { get; set; }
        public string? KLCL4 { get; set; }
        public string? KLCL5 { get; set; }

        // HuCL (0..5)
        public string? HuCL0 { get; set; }
        public string? HuCL1 { get; set; }
        public string? HuCL2 { get; set; }
        public string? HuCL3 { get; set; }
        public string? HuCL4 { get; set; }
        public string? HuCL5 { get; set; }

        // KLXi (0..4)
        public string? KLXi0 { get; set; }
        public string? KLXi1 { get; set; }
        public string? KLXi2 { get; set; }
        public string? KLXi3 { get; set; }
        public string? KLXi4 { get; set; }

        // KLPG (0..7)
        public string? KLPG0 { get; set; }
        public string? KLPG1 { get; set; }
        public string? KLPG2 { get; set; }
        public string? KLPG3 { get; set; }
        public string? KLPG4 { get; set; }
        public string? KLPG5 { get; set; }
        public string? KLPG6 { get; set; }
        public string? KLPG7 { get; set; }

        public string? KLNuoc { get; set; }
        public string? HoanThanh { get; set; }
        public int Flags { get; set; }

        // ------------------------------
        // Helpers: lấy property theo index
        // ------------------------------
        private string? GetKLCL(int i) => i switch
        {
            0 => KLCL0,
            1 => KLCL1,
            2 => KLCL2,
            3 => KLCL3,
            4 => KLCL4,
            5 => KLCL5,
            _ => null
        };

        private void SetKLCL(int i, string? v)
        {
            switch (i)
            {
                case 0: KLCL0 = v; break;
                case 1: KLCL1 = v; break;
                case 2: KLCL2 = v; break;
                case 3: KLCL3 = v; break;
                case 4: KLCL4 = v; break;
                case 5: KLCL5 = v; break;
            }
        }

        private void SetHuCL(int i, string? v)
        {
            switch (i)
            {
                case 0: HuCL0 = v; break;
                case 1: HuCL1 = v; break;
                case 2: HuCL2 = v; break;
                case 3: HuCL3 = v; break;
                case 4: HuCL4 = v; break;
                case 5: HuCL5 = v; break;
            }
        }

        private void SetKLXi(int i, string? v)
        {
            switch (i)
            {
                case 0: KLXi0 = v; break;
                case 1: KLXi1 = v; break;
                case 2: KLXi2 = v; break;
                case 3: KLXi3 = v; break;
                case 4: KLXi4 = v; break;
            }
        }

        private void SetKLPG(int i, string? v)
        {
            switch (i)
            {
                case 0: KLPG0 = v; break;
                case 1: KLPG1 = v; break;
                case 2: KLPG2 = v; break;
                case 3: KLPG3 = v; break;
                case 4: KLPG4 = v; break;
                case 5: KLPG5 = v; break;
                case 6: KLPG6 = v; break;
                case 7: KLPG7 = v; break;
            }
        }

        // ------------------------------
        // FromDHMeDO
        // ------------------------------
        public void FromDHMeDO(DHMeDO me, int CLDigit = 0, int XMDigit= 0, int PGDigit = 1, int NuocDigit = 0)
        {
            M3Tron = me.M3Tron.ToString("F3");

            for (int i = 0; i < me.KLCL.Length; i++)
                SetKLCL(i, Math.Round(me.KLCL[i], CLDigit).ToString());

            for (int i = 0; i < me.KLXi.Length; i++)
                SetKLXi(i, Math.Round(me.KLXi[i], XMDigit).ToString());

            for (int i = 0; i < me.KLPG.Length; i++)
                SetKLPG(i, Math.Round(me.KLPG[i], PGDigit).ToString());

            KLNuoc = Math.Round(me.KLNuoc, NuocDigit).ToString();
            HoanThanh = me.CreateAt?.ToString("HH:mm:ss");
        }

        // ------------------------------
        // FromPhieuTongKL
        // ------------------------------
        public void FromPhieuTongKL(DHPhieuVM ph, int CLDigit = 0, int XMDigit = 0, int PGDigit = 1, int NuocDigit = 0)
        {
            M3Tron = "Tổng";

            for (int i = 0; i < 5; i++)
                SetKLCL(i, Math.Round(ph.TongKLTheoSilos[i], CLDigit).ToString());

            for (int i = 0; i < 5; i++)
                SetKLXi(i, Math.Round(ph.TongKLTheoSilos[5 + i], XMDigit).ToString());

            for (int i = 0; i < 8; i++)
                SetKLPG(i, Math.Round(ph.TongKLTheoSilos[10 + i], PGDigit).ToString());

            KLNuoc = Math.Round(ph.TongKLTheoSilos[18], NuocDigit).ToString();
        }

        // ------------------------------
        // FromDHCongThuc
        // ------------------------------
        public void TenNLFromDHCongThuc(DHCongThucVM ct)
        {
            M3Tron = ct.Ma;

            foreach (var tp in ct.DsThanhPhan)
            {
                if (tp.NL_PhanLoai == Core.LoaiThanhPhan.CotLieu && tp.NL_SiloIndex < 5)
                    SetKLCL(tp.NL_SiloIndex, tp.NL_Ten);

                else if (tp.NL_PhanLoai == Core.LoaiThanhPhan.XiMang && tp.NL_SiloIndex < 5)
                    SetKLXi(tp.NL_SiloIndex, tp.NL_Ten);

                else if (tp.NL_PhanLoai == Core.LoaiThanhPhan.PhuGia && tp.NL_SiloIndex < 8)
                    SetKLPG(tp.NL_SiloIndex, tp.NL_Ten);
            }

            KLNuoc = "Nước";
        }
        public void CapPhoiFromDHCongThuc(DHCongThucVM ct, string ten, double m3 = 1, int CLDigit = 0, int XMDigit = 0, int PGDigit = 1, int NuocDigit = 0)
        {
            M3Tron = ten;

            foreach (var tp in ct.DsThanhPhan)
            {
                if (tp.NL_PhanLoai == Core.LoaiThanhPhan.CotLieu && tp.NL_SiloIndex < 5)
                    SetKLCL(tp.NL_SiloIndex, Math.Round(tp.KLCongThuc * m3, CLDigit).ToString());

                else if (tp.NL_PhanLoai == Core.LoaiThanhPhan.XiMang && tp.NL_SiloIndex < 5)
                    SetKLXi(tp.NL_SiloIndex, Math.Round(tp.KLCongThuc * m3, XMDigit).ToString());

                else if (tp.NL_PhanLoai == Core.LoaiThanhPhan.PhuGia && tp.NL_SiloIndex < 8)
                    SetKLPG(tp.NL_SiloIndex, Math.Round(tp.KLCongThuc * m3, PGDigit).ToString());
            }

            KLNuoc = Math.Round(ct.KLNuoc * m3, NuocDigit).ToString();
        }
    }
}
