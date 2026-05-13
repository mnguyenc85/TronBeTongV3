using MySqlConnector;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.Business;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.CSDL
{
    public sealed class DbRepository
    {
        private DbBridge _db = DbBridge.Instance;

        #region Singleton
        private static DbRepository? _instance;
        public static DbRepository Instance { get { return _instance ??= new DbRepository(); } }
        private DbRepository()
        {
            DataPath = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            ReportsPath = AppDomain.CurrentDomain.BaseDirectory + "reports\\";
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(ReportsPath)) Directory.CreateDirectory(ReportsPath);
        }
        #endregion

        public string DataPath;
        public string ReportsPath;

        public IPAddrDO PlcAddr { get; private set; } = new IPAddrDO();

        public long TongSoPhieu { get; set; }
        public long TongSoDon { get; set; }

        public async Task LoadDataAsync()
        {
            await PM_LoadAllStringsAsync();

            await KD_LoadAllAsync();

            await NguyenLieu_LoadAll();
            await CongThuc_LoadAll();

            await DonHang_LoadOnline();

            var conn = await _db.OpenConnAsync();
            TongSoDon = await _db.HT_DonHang_GetTotalOrdersInDayAsync(conn, DateTime.Now);
            string? sophieucuoi = await _db.HT_Phieu_GetLastSoPhieuAsync(conn);
            if (sophieucuoi == null) TongSoPhieu = 0;
            else
            {
                if (long.TryParse(sophieucuoi, out long spc)) TongSoPhieu = spc;
                else TongSoPhieu = 0;
            }
            await _db.CloseConnAsync(conn);
        }

        #region Phần mềm
        public DbSettings Settings { get; private set; } = new();
        public ObservableCollection<StringDO> DsPmStrings { get; private set; } = [];
        public Dictionary<int, StringDO> DictionaryPmStrings { get; private set; } = [];
        private int _max_id = -1;

        public async Task PM_LoadAllStringsAsync(string? cond = null)
        {
            var lst = await _db.PM_Strings_SelectAsync(cond);
            foreach (var i in lst)
            {
                DsPmStrings.Add(i);
                DictionaryPmStrings[i.Id] = i;
                if (_max_id < i.Id) _max_id = i.Id;
            }
        }

        #endregion

        #region Kinh doanh
        public ObservableCollection<KDKhachHangVM> DsKH { get; private set; } = [];
        public Dictionary<int, KDKhachHangVM> DictionaryKH { get; private set; } = [];
        
        public ObservableCollection<KDDuAnVM> DsDA { get; private set; } = [];
        public Dictionary<int, KDDuAnVM> DictionaryDA { get; private set; } = [];

        public ObservableCollection<KDXeLaiXeVM> DsXLx { get; private set; } = [];
        public ObservableCollection<KDXeVM> DsXe { get; private set; } = [];
        public Dictionary<int, KDXeVM> DictionaryXe { get; private set; } = [];
        public ObservableCollection<KDLaiXeVM> DsLaiXe { get; private set; } = [];
        public Dictionary<int, KDLaiXeVM> DictionaryLaiXe { get; private set; } = [];

        public void AddKhachHang(KDKhachHangVM kh)
        {
            if (kh.Id > 0)
            {
                DsKH.Add(kh);
                DictionaryKH[kh.Id] = kh;
            }
        }

        public void AddDuAn(KDDuAnVM da)
        {
            if (da.Id > 0)
            {
                DsDA.Add(da);
                DictionaryDA[da.Id] = da;
            }
        }

        public void AddXe(KDXeVM xe)
        {
            if (xe.Id > 0 && !DictionaryXe.ContainsKey(xe.Id))
            {
                DsXe.Add(xe);
                DictionaryXe[xe.Id] = xe;
            }
        }

        public void AddLaiXe(KDLaiXeVM lx)
        {
            if (lx.Id > 0 && !DictionaryLaiXe.ContainsKey(lx.Id))
            {
                DsLaiXe.Add(lx);
                DictionaryLaiXe[lx.Id] = lx;
            }
        }

        public async Task AddXeLaiXe(KDXeVM xe, KDLaiXeVM lx)
        {
            if (xe.Id > 0 && lx.Id > 0)
            {
                var xlx = DsXLx.FirstOrDefault(x => x.XeId == xe.Id && x.LaiXeId == lx.Id);
                if (xlx != null)
                {
                    if (xlx.Xe != null) xlx.Xe.CopyFrom(xe);
                    else
                    {
                        xlx.XeId = xe.Id;
                        xlx.Xe = xe;
                    }
                    if (xlx.LaiXe != null) xlx.LaiXe.CopyFrom(lx);
                    else
                    {
                        xlx.LaiXeId = lx.Id;
                        xlx.LaiXe = lx;
                    }
                }
                else
                {
                    await _db.XLxInsertAsync(xe.Id, lx.Id);
                    var lx1 = new KDLaiXeVM(); lx1.CopyFrom(lx);
                    var xe1 = new KDXeVM(); xe1.CopyFrom(xe);

                    AddXe(xe1);
                    AddLaiXe(lx1);
                    DsXLx.Add(new KDXeLaiXeVM()
                    {
                        LaiXeId = lx1.Id,
                        LaiXe = lx1,
                        XeId = xe1.Id,
                        Xe = xe1
                    });
                }
            }

        }

        private KDDuAnVM CreateDuAnVM(KDDuAnDO o)
        {
            KDDuAnVM vm = new KDDuAnVM();

            vm.Id = o.Id;
            if (DictionaryPmStrings.TryGetValue(o.DuAnId, out var sda)) vm.DuAn = sda.VanBan;
            if (DictionaryPmStrings.TryGetValue(o.CongTrinhId, out var sct)) vm.CongTrinh = sct.VanBan;
            if (DictionaryPmStrings.TryGetValue(o.HangMucId, out var shm)) vm.HangMuc = shm.VanBan;
            if (DictionaryPmStrings.TryGetValue(o.DiaChiId, out var sdc)) vm.DiaChi = sdc.VanBan;
            if (DictionaryKH.TryGetValue(o.KHId, out var kh)) vm.KHMa = kh.Ten;
            vm.GhiChu = o.GhiChu;
            vm.KHId= o.KHId;
            
            return vm;
        }

        private async Task KD_LoadAllAsync()
        {
            var lstKH = await _db.KhachHangSelectLimitAsync(0, 1000);
            foreach (var i in lstKH)
            {
                AddKhachHang(new KDKhachHangVM(i));
            }

            var lstXe = await _db.Xe_SelectLimitAsync(0, 1000);
            foreach (var i in lstXe)
            {
                var vm = new KDXeVM(i);
                DsXe.Add(vm);
                DictionaryXe[i.Id] = vm;
            }
            var lstLX = await _db.LaiXeSelectLimitAsync(0, 1000);
            foreach (var i in lstLX)
            {
                var vm = new KDLaiXeVM(i);
                DsLaiXe.Add(vm);
                DictionaryLaiXe[i.Id] = vm;
            }
            var lstXLx = await _db.XLxSelectLimitAsync(0, 1000);
            foreach (var i in lstXLx)
            {
                if (DictionaryXe.ContainsKey(i.XeId) && DictionaryLaiXe.ContainsKey(i.LaiXeId))
                {
                    DsXLx.Add(new KDXeLaiXeVM()
                    {
                        XeId = i.XeId,
                        Xe = DictionaryXe[i.XeId],
                        LaiXeId = i.LaiXeId,
                        LaiXe = DictionaryLaiXe[i.LaiXeId]
                    });
                }
            }
            
            var lstDA = await _db.KD_DuAn_SelectLimitAsync(0, 1000);
            foreach (var i in lstDA)
            {
                AddDuAn(CreateDuAnVM(i));
            }
        }
        #endregion

        #region Nguyen lieu
        public ObservableCollection<SiloNguyenLieuVM> DsNguyenLieu { get; private set; } = [];
        public Dictionary<int, SiloNguyenLieuVM> DicNL { get; private set; } = [];
        public async Task<bool> NguyenLieu_Save(SiloNguyenLieuVM nl)
        {
            var o = nl.CreateDO();
            await _db.NguyenLieu_SaveProcAsync(o);
            if (o.Id > 0)
            {
                if (nl.Id > 0)
                {
                    if (DicNL.TryGetValue(nl.Id, out var nl1))
                        nl1.CopyFrom(nl);
                }
                else
                {
                    nl.Id = o.Id;
                    var vm = new SiloNguyenLieuVM(o);
                    DsNguyenLieu.Add(vm);
                    DicNL[vm.Id] = vm;
                }
                nl.IsChanged = false;
                return true;
            }
            return false;
        }

        private async Task NguyenLieu_LoadAll()
        {
            var lstNL = await _db.NguyenLieu_SelectLimitAsync(0, 1000);
            foreach (var i in lstNL)
            {
                var vm = new SiloNguyenLieuVM(i);
                DsNguyenLieu.Add(vm);
                DicNL[vm.Id] = vm;
            }
        }
        #endregion

        #region Cong thuc
        public ObservableCollection<BTCongThucVM> DsCongThuc { get; private set; } = [];

        public async Task CongThuc_Save(BTCongThucVM ct)
        {
            if (ct.Id <= 0 || ct.IsChanged)
            {
                var ch = CauHinhTramTron.Instance;

                // Lấy kl nước từ thành phần
                foreach (var tp in ct.DsThanhPhan)
                {
                    if (tp.NL?.PhanLoai == Core.LoaiThanhPhan.Nuoc) { ct.KLNuoc = tp.KL; break; }
                }

                var o = ct.CreateDO();

                await _db.BTCongThuc_SaveAsync(o);
                BTCongThucVM? ct1 = null;
                if (ct.Id > 0)
                {
                    ct1 = DsCongThuc.FirstOrDefault(x => x.Id == ct.Id);
                    if (ct1 != null)
                    {
                        ct1.FromDO(o);
                        System.Diagnostics.Debug.WriteLine($"Update tp của ct Id = {ct.Id}");
                        CongThuc_UpdateThanhPhan(ct, ct1);
                    }
                }
                else
                {
                    ct.Id = o.Id;
                    ct1 = new BTCongThucVM(o)
                    {
                        STT = DsCongThuc.Count + 1
                    };
                    DsCongThuc.Add(ct1);

                    System.Diagnostics.Debug.WriteLine($"Save tp của ct Id = {ct.Id}");

                    await _db.BTThanhPhan_SaveNewAsync(ct);
                }

                if (ct1 != null)
                {
                    await CongThuc_LoadThanhPhan(ct1);
                    ch.CheckCongThuc(ct1);
                    ct1.KLSilos = ch.TinhKLTheoSilo(ct1);
                }

                ct.IsChanged = false;
            }
        }

        /// <summary>
        /// Thêm, sửa, xóa các thành phần trong công thức
        /// </summary>
        private async void CongThuc_UpdateThanhPhan(BTCongThucVM ct, BTCongThucVM ct0)
        {
            foreach (var tp in ct.DsThanhPhan) {
                // Lưu kl nước riêng!
                if (tp.NL?.PhanLoai != Core.LoaiThanhPhan.Nuoc)
                {
                    if (tp.State == Core.ViewModelStates.Add)
                    {
                        var o = tp.CreateDO(ct.Id);
                        await _db.BTThanhPhan_SaveAsync(o);
                        tp.Id = o.Id;
                        tp.State = Core.ViewModelStates.None;
                    }
                    else if (tp.State == Core.ViewModelStates.Remove && tp.Id > 0)
                    {
                        await _db.BTThanhPhan_DeleteAsync(tp.Id);
                    }
                    else
                    {
                        var tp1 = ct0.DsThanhPhan.FirstOrDefault(x => x.Id == tp.Id);
                        if (tp1 != null && tp1.KL != tp.KL)
                        {
                            var o = tp.CreateDO(ct.Id);
                            await _db.BTThanhPhan_SaveAsync(o);
                            tp.Id = o.Id;
                        }
                    }
                }
            }

            CongThuc_Clear(ct);
        }

        public void CongThuc_Clear(BTCongThucVM ct)
        {
            int i = 0;
            var tps = ct.DsThanhPhan;
            while (i < tps.Count)
            {
                if (tps[i].State == Core.ViewModelStates.Remove)
                    ct.DsThanhPhan.RemoveAt(i);
                else
                    i++;
            }
        }

        private async Task CongThuc_LoadAll()
        {
            var lst = await _db.BTCongThuc_SelectAllAsync();
            int n = 0;
            foreach (var i in lst)
            {
                var vm = new BTCongThucVM(i)
                {
                    STT = ++n
                };
                DsCongThuc.Add(vm);
            }
        }

        public async Task CongThuc_LoadThanhPhan(BTCongThucVM ct)
        {
            ct.DsThanhPhan.Clear();

            int n = 0;
            var lst = await _db.BTThanhPhan_SelectByCTIdAsync(ct.Id);
            foreach (var i in lst)
            {
                if (DicNL.TryGetValue(i.NLId, out SiloNguyenLieuVM? value))
                {
                    BTThanhPhanVM tp1 = new()
                    {
                        STT = ++n,
                        Id = i.Id,
                        NL = value,
                        KL = i.KL,
                    };
                    ct.DsThanhPhan.Add(tp1);
                }
            }
            ct.DsThanhPhan.Add(new BTThanhPhanVM()
            {
                STT = ++n,
                Id = -1,
                NL = new SiloNguyenLieuVM() { Ma = "Nuoc", Ten = "Nước", PhanLoai = Core.LoaiThanhPhan.Nuoc },
                KL = ct.KLNuoc
            });
        }

        public async Task CongThuc_LoadThanhPhan(DHCongThucVM ct)
        {
            ct.DsThanhPhan.Clear();

            int n = 0;
            var lst = await _db.BTThanhPhan_SelectByCTIdAsync(ct.Id);
            foreach (var i in lst)
            {
                if (DicNL.TryGetValue(i.NLId, out SiloNguyenLieuVM? value))
                {
                    DHThanhPhanVM tp1 = new()
                    {
                        STT = ++n,
                        Id = -1,                    // Lấy -1 vì sẽ lưu ở bảng khác
                        NL_Ma = value.Ma,
                        NL_Ten = value.Ten,
                        NL_PhanLoai = value.PhanLoai,
                        KLCongThuc = i.KL,
                    };
                    ct.DsThanhPhan.Add(tp1);
                }
            }

            // Nước
            DHThanhPhanVM nuoc = new()
            {
                STT = ++n,
                Id = -1,
                NL_Ma = "Nuoc",
                NL_Ten = "Nước",
                NL_PhanLoai = Core.LoaiThanhPhan.Nuoc,
                KLCongThuc = ct.KLNuoc,
            };
            ct.DsThanhPhan.Add(nuoc);
        }
        
        public async Task CongThuc_Delete(int id)
        {
            await _db.BTCongThuc_ProcDeleteAsync(id);
            var ct = DsCongThuc.FirstOrDefault(x => x.Id == id);
            if (ct != null) DsCongThuc.Remove(ct);
        }
        #endregion

        #region Đơn hàng
        public ObservableCollection<DHDonVM> DsDonOnline { get; private set; } = [];

        private async Task DonHang_LoadOnline()
        {
            int stt = 0;
            await foreach (var i in _db.HT_DonHang_SelectOnlineAsync())
            {
                var dhvm = new DHDonVM(i)
                {
                    DuAn = new KDDuAnVM(),
                    STT = ++stt
                };

                if (DictionaryKH.TryGetValue(i.KhachHangId, out var kh))
                    dhvm.KhachHang = kh;

                if (DictionaryPmStrings.TryGetValue(i.DuAnId, out var da))
                {
                    dhvm.DuAn.DuAn = da.VanBan;
                }
                if (DictionaryPmStrings.TryGetValue(i.CongTrinhId, out var ct))
                {
                    dhvm.DuAn.CongTrinh = ct.VanBan;
                }
                if (DictionaryPmStrings.TryGetValue(i.HangMucId, out var hm))
                {
                    dhvm.DuAn.HangMuc = hm.VanBan;
                }
                if (DictionaryPmStrings.TryGetValue(i.DiaChiId, out var dc))
                {
                    dhvm.DuAn.DiaChi = dc.VanBan;
                }

                DsDonOnline.Add(dhvm);
            }
        }

        public async Task DonHang_Save(DHDonVM don)
        {
            don.Ma = $"{DateTime.Now:yyyyMMdd}-{++TongSoDon}";
            var o = don.CreateDO();
            await _db.HT_DonHang_ProcSaveAsync(o);
            don.Id = o.Id;

            await PM_LoadAllStringsAsync($"id > {_max_id}");

            DsDonOnline.Insert(0, don.Clone());
            for (int i = 0; i < DsDonOnline.Count; i++)
                DsDonOnline[i].STT = ++i;
        }

        /// <summary>
        /// Lưu công thức -> lưu phiếu
        /// </summary>
        /// <returns>Id của bản ghi</returns>
        public async Task<int> Phieu_Save(MySqlConnection conn, DHPhieuVM ph)
        {
            if (ph.DonHang == null) return -1;

            // TODO: chuyển sang tạo phiếu Lưu công thức
            if (ph.CongThuc == null) { return -1; }

            // Lưu thành phần:
            List<int> tpids = [];
            foreach (var tp in ph.CongThuc.DsThanhPhan)
            {
                var tpdo = tp.CreateDO();
                await _db.HT_Phieu_CongThuc_ThanhPhan_SaveAsync(conn, ph.CongThuc.Id, tpdo);
                tp.Id = tpdo.Id;
                tpids.Add(tp.Id);
            }

            // Lưu công thức
            if (ph.CongThuc.Id < 0)
            {
                ph.CongThuc.SoTP = ph.CongThuc.DsThanhPhan.Count;
                var ctdo = ph.CongThuc.CreateDO();
                await _db.HT_Phieu_CongThuc_ProcSaveAsync(conn, ctdo, string.Join(",", tpids));
                ph.CongThuc.Id = ctdo.Id;                            
            }

            // Lưu quan hệ công thức - thành phần
            await _db.HT_CongThuc_ThanhPhan_SaveRelsAysnc(conn, ph.CongThuc.Id, tpids.ToArray());

            // Lưu phiếu
            var o = ph.CreateDO();
            await _db.HT_Phieu_SaveAsync(conn, o);

            if (o.Id > 0)
            {
                var d = DsDonOnline.FirstOrDefault(x => x.Id == ph.DonHang.Id);
                if (d != null) d.TongSoPhieu++;
            }
            return o.Id;
        }

        public async Task DonHang_LoadAllPhieu(DHDonVM don)
        {
            don.DsPhieu.Clear();

            var conn = await _db.OpenConnAsync();

            var lstphieu = await _db.HT_Phieu_LoadByDH(conn, don.Id);
            int stt = 0;
            double tichluy = 0;
            foreach (var ph in lstphieu)
            {
                var vm = await Phieu_GetContent(conn, ph);
                vm.STT = ++stt;
                tichluy += vm.TheTichHT;
                vm.TheTichTichLuy = tichluy;
                don.AddPhieu(vm);
            }
            don.TongSoPhieu = don.DsPhieu.Count;

            await _db.CloseConnAsync(conn);
        }

        public async Task<DHPhieuVM?> DonHang_LoadUnfinishedPhieu(DHDonVM don)
        {
            List<DHPhieuVM> lst = [];

            var conn = await _db.OpenConnAsync();

            var lstphieu = await _db.HT_Phieu_GetUnfinishedByDH(conn, don.Id);
            foreach (var ph in lstphieu)
            {
                var vm = await Phieu_GetContent(conn, ph);
                lst.Add(vm);
            }

            await _db.CloseConnAsync(conn);

            if (lst.Count > 0) return lst[0];
            return null;
        }

        private async Task<DHPhieuVM> Phieu_GetContent(MySqlConnection conn, DHPhieuDO ph)
        {
            var vm = new DHPhieuVM();
            vm.FromDO(ph);
            if (DictionaryXe.TryGetValue(ph.XeId, out var xe))
            {
                vm.Xe = xe;
            }
            if (DictionaryLaiXe.TryGetValue(ph.LaiXeId, out var lx))
            {
                vm.LaiXe = lx;
            }

            var ct = await _db.HT_Phieu_CongThuc_LoadByIdAsync(conn, ph.CongThucId);
            if (ct != null)
            {
                vm.CongThuc = new DHCongThucVM(ct);
                int n = 0;
                await foreach (var tp in _db.HT_Phieu_CongThuc_ThanhPhan_LoadAsync(conn, ct.Id))
                {
                    var tpvm = new DHThanhPhanVM(tp)
                    {
                        STT = ++n
                    };
                    vm.CongThuc.DsThanhPhan.Add(tpvm);
                }
                vm.CongThuc.SoTP = vm.CongThuc.DsThanhPhan.Count;
            }

            return vm;
        }
        #endregion
    }
}
