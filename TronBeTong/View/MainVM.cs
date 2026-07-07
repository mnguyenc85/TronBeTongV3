using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NMWPFControls.Controls;
using NMWPFControls.Core;
using NMWPFControls.Core.MVVM;
using TronBeTongV3.Comm;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel.DonHang;
using TronBeTongV3.Reports;

namespace TronBeTongV3.View
{
    public class MainVM: VMBase
    {
        private const bool _CHOTTUNGME = true;

        private DbBridge _db = DbBridge.Instance;
        private DbRepository _r = DbRepository.Instance;

        public ModelHeThong? TramTron { get; set; }

        public CtrlMessages? CtrlMsg { get; set; }

        #region Đơn hàng
        public DelayUpdate<string> DonOnlineFilter { get; private set; }
        private ObservableCollection<DHDonVM> DsDonOnline { get; set; }
        private CollectionViewSource _cvsDsDon = new();
        public ICollectionView CVDsDonOnline { get { return _cvsDsDon.View; } }

        public DHDonVM? ActiveDon { get; set; }

        private DHPhieuVM? _ph;
        /// <summary>
        /// Phiếu đang sử dụng. Mẻ trộn sẽ lưu vào đây
        /// </summary>
        public DHPhieuVM? ActivePhieu { get { return _ph; } set { if (_ph != value) { _ph = value; NotifyChanged(); } } }

        public List<DHMeDO> DsMe { get; set; } = [];
        #endregion

        #region CmdClearOnlineOrder
        public RelayCommand CmdClearOnlineOrder { get; private set; }
        private bool CanClearOnlineOrder(object obj)
        {
            return _active_lv > 0;
        }

        private void ExeClearOnlineOrder(object obj)
        {
            ClearActiveOrder();
            DonOnlineFilter.Run(_last_filter_string);
        }
        private string? _last_filter_string;
        #endregion

        public double KLMinLuuMe { get; set; } = 0;


        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public MainVM() {
            DsDonOnline = _r.DsDonOnline;
            _cvsDsDon.Source = DsDonOnline;

            DonOnlineFilter = new(300)
            {
                ExecUpdate = f =>
                {
                    _last_filter_string = f;
                    CVDsDonOnline.Filter = p =>
                    {
                        if (p is DHDonVM d)
                        {
                            if (d.ActiveLv < _active_lv) return false;
                            return (string.IsNullOrEmpty(f) || d.KhachHang?.Ten?.Contains(f, StringComparison.CurrentCultureIgnoreCase) == true ||
                                (d.DuAn != null &&
                                (d.Ma?.Contains(f, StringComparison.CurrentCultureIgnoreCase) == true ||
                                 d.DuAn.DuAn?.Contains(f, StringComparison.CurrentCultureIgnoreCase) == true || 
                                 d.DuAn.CongTrinh?.Contains(f, StringComparison.CurrentCultureIgnoreCase) == true || 
                                 d.DuAn.HangMuc?.Contains(f, StringComparison.CurrentCultureIgnoreCase) == true)));
                        }
                        return false;
                    };
                }
            };

            CmdClearOnlineOrder = new RelayCommand(ExeClearOnlineOrder, CanClearOnlineOrder);
        }

        public void ClearActivePhieu()
        {
            ActivePhieu = null;
            DsMe.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ph">Lưu thông tin phiếu do người nhập</param>
        /// <param name="forceNew">Bắt buộc tạo phiếu mới</param>
        /// <returns></returns>
        public int SetActivePhieu(DHPhieuVM ph, bool forceNew)
        {
            if (ph.DonHang == null)
            {
                //MessageBox.Show("Lỗi: Chưa chọn đơn hàng", "Lưu phiếu", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
            else if (ph.CongThuc == null)
            {
                //MessageBox.Show("Lỗi: Chưa chọn công thức", "Lưu phiếu", MessageBoxButton.OK, MessageBoxImage.Error);
                return -2;
            }

            bool tangsophieu = ActivePhieu == null || (ActivePhieu.Id > 0 && ph.Id <= 0);

            bool phieumoi = forceNew || ActivePhieu == null
                            || ActivePhieu.CongThuc?.Ma != ph.CongThuc.Ma || ActivePhieu.TheTichDat != ph.TheTichDat || ActivePhieu.MeDat != ph.MeDat;
                            
            // || ActivePhieu.Flags > 0; quên mất tại sao lại kiểm tra cái này

            int ret = 0;
            if (phieumoi)
            {
                ActivePhieu = ph.Clone();
                ActivePhieu.TGBD = DateTime.Now;
                ActivePhieu.DonHang = ph.DonHang;
                if (tangsophieu) 
                    ActivePhieu.SoPhieu = (++_r.TongSoPhieu).ToString();
                else 
                    ActivePhieu.SoPhieu = (_r.TongSoPhieu).ToString();
                ActivePhieu.UpdateTrangThai();
                DsMe.Clear();
                ret = 1;
            }

            SetTTDonHang(2);

            return ret;
        }

        public async Task<int> SaveMe(DHMeDO me)
        {
            int ret = 0;
            var conn = await _db.OpenConnAsync();

            if (ActivePhieu != null)
            {
                bool dungTay = (me.Flags & 2) == 2;
                bool memoi = me.Id <= 0;

                if (me.CheckEmpty(KLMinLuuMe)) ret = -1;
                else
                {
                    if (ActivePhieu.Id <= 0)
                    {
                        ActivePhieu.TGHT = DateTime.Now;
                        if (ActivePhieu.DonHang != null)
                        {
                            ActivePhieu.DonStt = ActivePhieu.DonHang.TongSoPhieu + 1;
                            ActivePhieu.STT = ActivePhieu.DonStt;
                            ActivePhieu.DonM3 = Math.Round(ActivePhieu.DonHang.TheTichHT, 3);
                        }
                        ActivePhieu.Id = await _r.Phieu_Save(conn, ActivePhieu);
                    }
                    else
                    {
                        ActivePhieu.TGHT = DateTime.Now;
                        double tklme = me.TongKL();
                        double klthme = Math.Round(tklme - me.KLDaLuuPhieu, 2);
                        ActivePhieu.KLHT += klthme;
                        me.KLDaLuuPhieu = tklme;
                        await _db.HT_Phieu_UpdateHoanThanhAsync(conn, ActivePhieu.CreateDO());
                        if (ActivePhieu.DonHang != null) 
                            ActivePhieu.DonHang.KLHT = Math.Round(ActivePhieu.DonHang.KLHT + klthme, 2);
                    }

                    me.PhieuId = ActivePhieu.Id;
                    if ((me.ChotNuoc && me.ChotCotLieu && me.ChotXi) || dungTay || _CHOTTUNGME)
                    {
                        if (CtrlMsg != null) CtrlMsg.AddMessage($"Lưu mẻ {me.STT}");
                        else System.Diagnostics.Debug.WriteLine($"Lưu mẻ {me.STT}");

                        await _db.Me_SaveAsync(conn, me);
                        if (dungTay) await _db.Me_SaveTTAsync(conn, me);

                        if (memoi)
                        {
                            if (me.Id > 0)
                            {
                                ActivePhieu.TheTichHT += me.M3Tron;
                                ActivePhieu.MeHT++;
                                if ((me.Flags & 4) == 4)
                                {
                                    // Mẻ mô phỏng
                                    ActivePhieu.DonM3Sim += me.M3Tron;
                                }
                                else ActivePhieu.DonM3 += me.M3Tron;
                                ActivePhieu.UpdateTrangThai();

                                await _db.HT_Phieu_UpdateHoanThanhAsync(conn, ActivePhieu.CreateDO());
                            }

                            if (ActivePhieu.DonHang != null)
                            {
                                ActivePhieu.DonHang.TheTichHT = Math.Round(ActivePhieu.DonHang.TheTichHT + ActivePhieu.TheTichMe, 3);
                                ActivePhieu.DonHang.MeHT++;
                            }
                        }
                        ret = 1;
                    }
                    else ret = -4;

                    if (!DsMe.Contains(me))
                    {
                        DsMe.Add(me);
                        if (CtrlMsg != null) CtrlMsg.AddMessage($"Thêm mẻ {me.STT} vào ds");
                        else System.Diagnostics.Debug.WriteLine($"Thêm mẻ {me.STT} vào ds");
                    }
                }
            }

            await _db.CloseConnAsync(conn);
            return ret;
        }

        public async void KetThucPhieu()
        {
            if (ActivePhieu == null) return;

            await _db.HT_Phieu_SetTrangThai(ActivePhieu.Id, 1);

            ActivePhieu = null;
        }

        /// <summary>
        /// Đặt trạng thái cho đơn hiện tại: 0: bình thường, 1 chọn, 2 đang trộn
        /// </summary>
        /// <param name="tt"></param>
        public void SetTTDonHang(int tt)
        {
            if (ActivePhieu != null && ActivePhieu.DonHang != null) ActivePhieu.DonHang.TrangThai = tt;
        }

        /// <summary>
        /// Dùng để lọc đơn
        /// </summary>
        private int _active_lv = 0;
        /// <summary>
        /// Đánh dấu các đơn hiện tại thành active
        /// </summary>
        public void MarkActiveOrder()
        {
            _active_lv++;
            foreach (var o in CVDsDonOnline)
            {
                if (o is DHDonVM d)
                {
                    d.ActiveLv = _active_lv;
                }
            }
        }

        public void ClearActiveOrder()
        {
            _active_lv = 0;
            foreach (var d in DsDonOnline)
            {
                d.ActiveLv = 0;
            }
        }

        public async void InPhieu(bool canPrint, int solan, string? kepchi)
        {
            if (!canPrint || ActivePhieu == null) return;

            ActivePhieu.KepChi = kepchi;

            await _db.HT_Phieu_UpdateUserInfoAsync(ActivePhieu.CreateDO());

            XuLyInPhieu _print = XuLyInPhieu.Instance;
            if (_print.ReportTemplate == null) _print.LoadTemplate();

            if (await _print.Print(ActivePhieu, DsMe, false, null, false, solan))
            {
                GlobalUIEvent.Instance.RaiseEvent(this, GlobalUIEventKinds.DebugMsg, $"In phiếu {ActivePhieu.SoPhieu}");
            }
        }
    }
}
