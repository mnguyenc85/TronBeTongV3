using System;
using System.Threading.Tasks;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.Data.DO
{
    /// <summary>
    /// Lưu câu hình của trạm trộn
    /// </summary>
    public class CauHinhTramTron
    {
        private DbBridge _db = DbBridge.Instance;

        /// <summary>
        /// Số thành phần cốt liệu tối đa
        /// </summary>
        public const int MAX_CL = 6;
        /// <summary>
        /// Số thành phần xi măng tối đa
        /// </summary>
        public const int MAX_XM = 5;
        /// <summary>
        /// Số thành phần phụ gia tối đa
        /// </summary>
        public const int MAX_PG = 8;

        public List<SiloCauHinhVM> DsSilos { get; private set; } = new List<SiloCauHinhVM>() { Capacity = 20 };

        public async Task Load()
        {
            var r = DbRepository.Instance;

            var lst = await _db.Silo_SelectAllAsync();
            foreach (var s in lst)
            {
                SiloCauHinhVM? silo = null;
                switch ((LoaiThanhPhan)s.PhanLoai)
                {
                    case LoaiThanhPhan.CotLieu:
                        silo = DsSilos[s.Index];
                        break;
                    case LoaiThanhPhan.XiMang:
                        silo = DsSilos[MAX_CL + s.Index];
                        break;
                    case LoaiThanhPhan.PhuGia:
                        silo = DsSilos[MAX_CL + MAX_XM + s.Index];
                        break;
                }

                if (silo != null)
                {
                    silo.Id = s.Id;
                    if (r.DicNL.TryGetValue(s.NLId, out var nl))
                    {
                        silo.NLId = s.NLId;
                        silo.NguyenLieu = nl;
                    }
                    silo.TSId = s.TSId;
                }
            }
        }

        public SiloCauHinhVM? GetCauHinh(LoaiThanhPhan loai, int i)
        {
            switch (loai)
            {
                case LoaiThanhPhan.CotLieu:
                    if (i >= 0 && i < MAX_CL) return DsSilos[i];
                    break;
                case LoaiThanhPhan.XiMang:
                    if (i >= 0 && i < MAX_XM) return DsSilos[MAX_CL + i];
                    break;
                case LoaiThanhPhan.PhuGia:
                    if (i >= 0 && i < MAX_PG) return DsSilos[MAX_CL + MAX_XM + i];
                    break;
                case LoaiThanhPhan.Nuoc:
                    // TODO:
                    break;
            }
            return null;
        }

        public int GetSiloIndex(LoaiThanhPhan loai, int i)
        {
            switch (loai)
            {
                case LoaiThanhPhan.CotLieu:
                    if (i >= 0 && i < MAX_CL)
                        return i;
                    break;
                case LoaiThanhPhan.XiMang:
                    if (i >= 0 && i < MAX_XM)
                        return MAX_CL + i;
                    break;
                case LoaiThanhPhan.PhuGia:
                    if (i >= 0 && i < MAX_PG)
                        return MAX_CL + MAX_XM + i;
                    break;
                case LoaiThanhPhan.Nuoc:
                    return MAX_CL + MAX_XM + MAX_PG;
            }
            return -1;
        }

        public async Task SetNguyenLieu(LoaiThanhPhan loai, int i, SiloNguyenLieuVM? nl)
        {
            int j = GetSiloIndex(loai, i);
            if (j >= 0)
            {
                if (nl != null)
                {
                    var nl1 = new SiloNguyenLieuVM();
                    nl1.CopyFrom(nl);
                    DsSilos[j].NguyenLieu = nl1;
                }
                else DsSilos[j].NguyenLieu = null;
                var o = DsSilos[j].CreateDO();
                await _db.Silo_SaveAsync(o);
                DsSilos[j].Id = o.Id;
            }
        }

        public void CheckCongThuc(BTCongThucVM ct)
        {
            foreach (var s in DsSilos)
            {
                s.Flags = 0;
            }

            foreach (var tp in ct.DsThanhPhan)
            {
                if (tp.NL != null) {
                    tp.LoaiSilo = LoaiThanhPhan.None;
                    if (tp.NL.Ma == "Nuoc")
                    {
                        tp.SiloTen = "Nước";
                        tp.LoaiSilo = LoaiThanhPhan.Nuoc;
                        tp.LoaiSiloIndex = 0;
                    }
                    else
                    {
                        foreach (var s in DsSilos)
                        {
                            if (s.NguyenLieu != null && s.Flags == 0 && tp.NL.Id == s.NguyenLieu.Id)
                            {
                                tp.SiloTen = s.Ten;
                                tp.LoaiSiloIndex = s.Index;
                                tp.LoaiSilo = s.PhanLoai;
                                s.Flags = 1;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public DHKgSilos TinhKLTheoSilo(BTCongThucVM ct)
        {
            DHKgSilos nl = new(MAX_CL, MAX_XM, MAX_PG);
            List<string> tenNLKhac = [];
            nl.KLKhac = 0;

            foreach (var tp in ct.DsThanhPhan)
            {
                switch (tp.LoaiSilo)
                {
                    case LoaiThanhPhan.CotLieu:
                        nl.CLs[tp.LoaiSiloIndex] = tp.KL;
                        break;
                    case LoaiThanhPhan.XiMang:
                        nl.Xis[tp.LoaiSiloIndex] = tp.KL;
                        break;
                    case LoaiThanhPhan.PhuGia:
                        nl.PGs[tp.LoaiSiloIndex] = tp.KL;
                        break;
                    case LoaiThanhPhan.Nuoc:
                        nl.Nuoc = tp.KL;
                        break;
                    case LoaiThanhPhan.None:
                        if (tp.NL != null && tp.NL.Ma != null)
                        {
                            nl.KLKhac += tp.KL;
                            tenNLKhac.Add(tp.NL.Ma);
                        }
                        break;
                }
            }
            nl.TenKhac = string.Join(", ", tenNLKhac);

            return nl;
        }

        public int CheckCongThuc(DHCongThucVM ct)
        {
            int missing = 0;

            foreach (var s in DsSilos) s.Flags = 0;

            foreach (var tp in ct.DsThanhPhan)
            {
                if (tp.NL_Ma == "Nuoc")
                {
                    tp.SiloTen = "Nước";
                    tp.NL_SiloIndex = 0;
                }
                else
                {
                    tp.NL_SiloIndex = -1;
                    foreach (var s in DsSilos)
                    {
                        if (s.NguyenLieu != null && s.Flags == 0 && tp.NL_Ma == s.NguyenLieu.Ma)
                        {
                            tp.SiloTen = s.Ten;
                            tp.NL_SiloIndex = s.Index;
                            s.Flags = 1;
                            break;
                        }
                    }
                    if (tp.NL_SiloIndex < 0) missing++;
                }
            }

            return missing;
        }

        #region Singleton
        private static CauHinhTramTron? _instance = null;
        private CauHinhTramTron() {
            int k = 0;
            for (int i = 0; i < MAX_CL; i++)
            {
                DsSilos.Add(new SiloCauHinhVM()
                {
                    STT = ++k,
                    Ten = $"Cốt liệu {i+1}",
                    Ma = $"CL{i+1}",
                    PhanLoai = LoaiThanhPhan.CotLieu,
                    Index = i,
                });
            }
            for (int i = 0; i < MAX_XM; i++)
            {
                DsSilos.Add(new SiloCauHinhVM()
                {
                    STT = ++k,
                    Ten = $"Xi măng {i + 1}",
                    Ma = $"XM{i + 1}",
                    PhanLoai= LoaiThanhPhan.XiMang,
                    Index = i,
                });
            }
            for (int i = 0; i < MAX_PG; i++)
            {
                DsSilos.Add(new SiloCauHinhVM()
                {
                    STT = ++k,
                    Ten = $"Phụ gia {i + 1}",
                    Ma = $"PG{i + 1}",
                    PhanLoai = LoaiThanhPhan.PhuGia,
                    Index = i,
                });
            }
            DsSilos.Add(new SiloCauHinhVM()
            {
                STT = ++k,
                Ten = "Nước",
                Ma = "Nuoc",
                PhanLoai = LoaiThanhPhan.Nuoc,
                NguyenLieu = new SiloNguyenLieuVM()
                {
                    Id = -1,
                    PhanLoai = LoaiThanhPhan.Nuoc,
                    Ten = "Nước",
                    Ma = "Nuoc",
                },
                Index = 0,
            });
        }
        public static CauHinhTramTron Instance => _instance ??= new CauHinhTramTron();
        #endregion
    }
}
