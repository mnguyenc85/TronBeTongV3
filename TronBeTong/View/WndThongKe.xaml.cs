using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.DO.ThongKe;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Reports;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for ThongKeWnd.xaml
    /// </summary>
    public partial class WndThongKe : Window, INotifyPropertyChanged
    {
        private DbBridge _db = DbBridge.Instance;
        private DbRepository _r = DbRepository.Instance;
        private StringBuilder _sb = new();
        private List<string> _vars = [];

        private XuLyInBaoCao _inbaocao = XuLyInBaoCao.Instance;

        private Dictionary<int, TKDonHangDO> _dictionaryDonHang = [];
        private List<TKThanhPhanDO> _uniqueMaTP = [];
        private Dictionary<int, TKCongThucDO> dictionaryCongThuc = [];

        public ObservableCollection<KDKhachHangVM> DsKhachHang { get; set; }

        private List<TKMeInDO> _kqThongKeMe = [];
        public ObservableCollection<TKMeInDO> KQMe { get; private set; } = [];

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndThongKe()
        {
            InitializeComponent();
            DataContext = this;

            DsKhachHang = _r.DsKH;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double appZoom = _r.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;

            DtEnd.Value = DtStart.Value + new TimeSpan(23, 59, 59);
            if (_inbaocao.ReportTemplate == null) _inbaocao.LoadTemplate();
        }

        private async void BtThongKe_Click(object sender, RoutedEventArgs e)
        {
            _kqThongKeMe.Clear();
            KQMe.Clear();

            bool theodonhang = await ThongKeDonHang();

            #region Tìm phiếu
            bool skipQuery = false;
            // Tính điều kiện
            _sb.Clear(); _vars.Clear();
            if (!string.IsNullOrEmpty(TxtSoPhieu.Text))
            {
                _sb.Append($"sophieu IN ({PhanTichSoPhieu(TxtSoPhieu.Text)})");
            }
            else
            {
                if (theodonhang)
                {
                    if (_dictionaryDonHang.Count > 0) _sb.Append($"donhang_id IN ({string.Join(",", _dictionaryDonHang.Keys)})");
                    else skipQuery = true;
                }
                if (!string.IsNullOrEmpty(TxtBSX.Text))
                {
                    if (_sb.Length > 0) _sb.Append(" AND ");
                    _sb.Append($"xe_id IN (SELECT id FROM kd_xe WHERE bsx LIKE @p{_vars.Count})");
                    _vars.Add($"%{TxtBSX.Text}%");
                }
                if (!string.IsNullOrEmpty(TxtLaiXe.Text))
                {
                    if (_sb.Length > 0) _sb.Append(" AND ");
                    _sb.Append($"lx_id IN (SELECT id FROM kd_laixe WHERE ten LIKE @p{_vars.Count})");
                    _vars.Add($"%{TxtLaiXe.Text}%");
                }
            }
            if (ChkTimeStart.IsChecked == true)
            {
                if (_sb.Length > 0) _sb.Append(" AND ");
                _sb.Append($"created_at >= @p{_vars.Count}");
                _vars.Add(DtStart.Value.ToString("yyyy/MM/dd HH:mm"));
            }
            if (ChkTimeEnd.IsChecked == true)
            {
                if (_sb.Length > 0) _sb.Append(" AND ");
                _sb.Append($"created_at < @p{_vars.Count}");
                _vars.Add(DtEnd.Value.ToString("yyyy/MM/dd HH:mm"));
            }

            if (skipQuery) return;

            // Tìm phiếu
            var lstphieu = await _db.ThongKe_PhieuAsync(_sb.ToString(), _vars.ToArray());
            HashSet<int> ctids = [];
            List<TKPhieuDO> lstTKPhieu = [];
            foreach (var ph in lstphieu)
            {
                ctids.Add(ph.CongThucId);
                lstTKPhieu.Add(new TKPhieuDO(ph));
            }
            #endregion

            await ThongKeCongThuc(ctids.ToArray());

            int flags = 0;
            if (OptAll.IsChecked == true) flags = 3;
            else if (OptGoodBatch.IsChecked == true) { flags = 1; }
            else if (OptBadBatch.IsChecked == true) { flags = 2; }
            foreach (var ph in lstTKPhieu)
            {
                await ph.TongHop(dictionaryCongThuc, _uniqueMaTP, flags, ChkSimBatch.IsChecked == true);
            }

            CreateDataGridColumns(Dgrid1, _uniqueMaTP);

            #region Xử lý dữ liệu -> TKMeDO
            HashSet<int> donhangids = [];
            HashSet<int> phieuids = [];
            HashSet<int> xeids = [];
            HashSet<int> laixeids = [];
            HashSet<int> congthucids = [];
            double tongM3 = 0;
            double tongSoMe = 0;
            double[] tongkl = new double[_uniqueMaTP.Count];
            TKMeInDO tktong = new()
            {
                MeId = -5,
                TGHT = "Tổng",
                Flags = 1024,
            };
            foreach (var ph in lstTKPhieu)
            {
                double phieum3 = 0;
                donhangids.Add(ph.DonHangId);
                phieuids.Add(ph.Id);
                xeids.Add(ph.XeId);
                laixeids.Add(ph.LaiXeId);
                congthucids.Add(ph.CongThucId);
                tongSoMe += ph.KLTheoTP.DsMe.Count;

                #region Cấp phối
                TKMeInDO tkcapphoi = new()
                {
                    DonId = ph.DonHangId,
                    PhieuId = ph.Id,
                    MeId = -1,
                    TGHT = ph.TGHT?.ToString("dd/MM/yyyy HH:mm"),
                    M3Tron = "1",
                    SoMe = "",
                    PhieuMa = ph.SoPhieu,
                    Flags = 256,
                };
                if (_dictionaryDonHang.TryGetValue(ph.DonHangId, out TKDonHangDO? don))
                {
                    tkcapphoi.DonMa = don.Ma;
                    tkcapphoi.DonKhachHang = don.KhachHangTen;
                    tkcapphoi.DonDuAn = don.DuAn;
                }
                if (_r.DictionaryXe.TryGetValue(ph.XeId, out KDXeVM? xe)) tkcapphoi.PhieuBSX = xe.BSX;
                if (_r.DictionaryLaiXe.TryGetValue(ph.LaiXeId, out KDLaiXeVM? lx)) tkcapphoi.PhieuLaiXe = lx.Ten;
                if (dictionaryCongThuc.TryGetValue(ph.CongThucId, out TKCongThucDO? ct)) tkcapphoi.PhieuCapPhoi = ct.Ma;

                if (ph.KLTheoTP.CTKLs != null)
                {
                    for (int i = 0; i < _uniqueMaTP.Count; i++)
                    {
                        if (i < ph.KLTheoTP.CTKLs.Length && ph.KLTheoTP.CTKLs[i] > 0)
                        {
                            tkcapphoi.SetTP(i, ph.KLTheoTP.CTKLs[i].ToString());
                        }
                    }
                }
                var tkcapphoi1 = tkcapphoi.Clone();
                tkcapphoi1.ClearTPs();
                tkcapphoi1.MeId = -2;
                tkcapphoi.M3Tron = "1";
                if (ph.KLTheoTP.CTKLs != null)
                {
                    for (int i = 0; i < _uniqueMaTP.Count; i++)
                    {
                        if (i < ph.KLTheoTP.CTKLs.Length && ph.KLTheoTP.CTKLs[i] > 0)
                        {
                            tkcapphoi.SetTP(i, ph.KLTheoTP.CTKLs[i].ToString());
                        }
                    }
                }
                _kqThongKeMe.Add(tkcapphoi1);
                _kqThongKeMe.Add(tkcapphoi);
                #endregion

                #region Chi tiết mẻ
                TKMeInDO tkme;
                // Mẻ
                foreach (var me in ph.KLTheoTP.DsMe)
                {
                    tkme = new()
                    {
                        DonId = ph.DonHangId,
                        PhieuId = ph.Id,
                        MeId = me.Id,
                        TGHT = me.TGHT?.ToString("dd/MM/yyyy HH:mm"),
                        M3Tron = me.M3Tron.ToString("F3"),
                        SoMe = me.STT.ToString(),
                        //PhieuMa = ph.SoPhieu,
                    };

                    for (int i = 0; i < me.KLs.Length; i++)
                    {
                        if (i < me.KLs.Length && me.KLs[i] > 0)
                            tkme.SetTP(i, me.KLs[i].ToString("F0"));
                    }

                    tkme.TrangThai = GetTTFromFlags(me.Flags);
                    tkme.Flags = me.Flags;

                    _kqThongKeMe.Add(tkme);
                    phieum3 += me.M3Tron;
                }
                #endregion

                #region Tổng khối lượng
                tkme = new()
                {
                    DonId = ph.DonHangId,
                    PhieuId = ph.Id,
                    MeId = -3,
                    TGHT = "Tổng",
                    M3Tron = phieum3.ToString("F3"),
                    SoMe = ph.KLTheoTP.DsMe.Count.ToString(),
                    //PhieuMa = ph.SoPhieu,
                    Flags = 128,
                };

                if (ph.KLTheoTP.TongKL != null)
                {
                    for (int i = 0; i < _uniqueMaTP.Count; i++)
                    {
                        if (i < ph.KLTheoTP.TongKL.Length && ph.KLTheoTP.TongKL[i] > 0)
                        { 
                            tkme.SetTP(i, ph.KLTheoTP.TongKL[i].ToString("F0"));
                            tongkl[i] += ph.KLTheoTP.TongKL[i];
                        }
                    }
                }
                _kqThongKeMe.Add(tkme);
                var thongketkl2 = new TKMeInDO()
                {
                    DonId = ph.DonHangId,
                    PhieuId = ph.Id,
                    MeId = -4,
                    DonMa = tkcapphoi.DonMa,
                    DonKhachHang = tkcapphoi.DonKhachHang,
                    DonDuAn = tkcapphoi.DonDuAn,
                    PhieuMa = tkcapphoi.PhieuMa,
                    PhieuBSX = tkcapphoi.PhieuBSX,
                    PhieuLaiXe = tkcapphoi.PhieuLaiXe,
                    PhieuCapPhoi = tkcapphoi.PhieuCapPhoi,
                    TGHT = tkcapphoi.TGHT,
                    M3Tron = phieum3.ToString("F3"),
                    SoMe = ph.KLTheoTP.DsMe.Count.ToString(),
                    Flags = 128
                };
                for (int i = 0; i < _uniqueMaTP.Count; i++)
                {
                    thongketkl2.SetTP(i, tkme.GetTP(i));
                }
                _kqThongKeMe.Add(thongketkl2);
                tongM3 += phieum3;
                #endregion
            }
            tktong.DonMa = donhangids.Count.ToString();
            tktong.PhieuMa = phieuids.Count.ToString();
            tktong.PhieuBSX = xeids.Count.ToString();
            tktong.PhieuLaiXe = laixeids.Count.ToString();
            tktong.PhieuCapPhoi = congthucids.Count.ToString();
            tktong.M3Tron = tongM3.ToString("F3");
            tktong.SoMe = tongSoMe.ToString();
            for (int i = 0; i < _uniqueMaTP.Count; i++)
                tktong.SetTP(i, Math.Round(tongkl[i], _uniqueMaTP[i].RoundDigit).ToString());
            _kqThongKeMe.Add(tktong);

            ShowKQMe(ChkChiTietMe.IsChecked == true, ChkCapPhoi.IsChecked == true);
            #endregion
        }

        /// <summary>
        /// Hiển thị lại kết quả ra listview
        /// </summary>
        /// <param name="chitietme">Hiển thị chi tiết từng mẻ</param>
        /// <param name="hiencapphoi">Hiển thị cấp phối chuẩn</param>
        public void ShowKQMe(bool chitietme, bool hiencapphoi)
        {
            KQMe.Clear();
            if (!chitietme && !hiencapphoi)
            {
                foreach (var me in _kqThongKeMe)
                {
                    if (me.MeId == -4 || me.MeId == -5) KQMe.Add(me);
                }
            }
            else
            {
                foreach (var me in _kqThongKeMe)
                {
                    if (me.MeId == -1) { if (hiencapphoi) KQMe.Add(me); }
                    else if (me.MeId == -2) { if (!hiencapphoi) KQMe.Add(me); }
                    else if (me.MeId >= 0)
                    {
                        if (chitietme) KQMe.Add(me);
                    }
                    else if (me.MeId == -3 || me.MeId == -5) KQMe.Add(me);
                }
            }
        }

        /// <summary>
        /// Tìm đơn hàng thỏa mãn điều kiện khách hàng, dự án
        /// </summary>
        /// <returns>Có tìm phiếu theo đơn hàng không?</returns>
        private async Task<bool> ThongKeDonHang()
        {
            _sb.Clear();
            _vars.Clear();
            _dictionaryDonHang.Clear();

            bool theodonhang = false;

            if (!string.IsNullOrEmpty(CboKH.Text))
            {
                _sb.Append($"kh_id IN (SELECT id FROM kd_khachhang WHERE ten LIKE @p{_vars.Count})");
                _vars.Add("%" + CboKH.Text + "%");
                theodonhang = true;
            }
            if (!string.IsNullOrEmpty(TxtMaDon.Text))
            {
                if (_sb.Length > 0) _sb.Append(" AND ");
                _sb.Append($"ma LIKE @p{_vars.Count}");
                _vars.Add("%" + TxtMaDon.Text + "%");
                theodonhang = true;
            }
            if (!string.IsNullOrEmpty(TxtHangMuc.Text))
            {
                if (_sb.Length > 0) _sb.Append(" AND ");
                _sb.Append($@"((da_id IN (SELECT id FROM pm_strings WHERE vanban LIKE @p{_vars.Count} AND phanloai=1)) 
                          OR (ct_id IN (SELECT id FROM pm_strings WHERE vanban LIKE @p{_vars.Count} AND phanloai=2))
                          OR (hm_id IN (SELECT id FROM pm_strings WHERE vanban LIKE @p{_vars.Count} AND phanloai=3)))");
                _vars.Add($"%{TxtHangMuc.Text}%");
                theodonhang = true;
            }
            //if (ChkTimeStart.IsChecked == true)
            //{
            //    if (_sb.Length > 0) _sb.Append(" AND ");
            //    _sb.Append($"created_at >= @p{_vars.Count}");
            //    _vars.Add(DtStart.Value.ToString("yyyy/MM/dd HH:mm"));
            //    theodonhang = true;
            //}
            //if (ChkTimeEnd.IsChecked == true)
            //{
            //    if (_sb.Length > 0) _sb.Append(" AND ");
            //    _sb.Append($"created_at < @p{_vars.Count}");
            //    _vars.Add(DtEnd.Value.ToString("yyyy/MM/dd HH:mm"));
            //    theodonhang = true;
            //}

            var lstdh = await _db.ThongKe_DonHangAsync(_sb.ToString(), _vars.ToArray());
            foreach (var dh in lstdh)
            {
                var tkdon = new TKDonHangDO() { Id = dh.Id, Ma = dh.Ma };
                if (_r.DictionaryKH.TryGetValue(dh.KhachHangId, out KDKhachHangVM? kh)) tkdon.KhachHangTen = kh.Ten;
                if (_r.DictionaryPmStrings.TryGetValue(dh.DuAnId, out Data.DO.Business.StringDO? sda)) tkdon.DuAn = sda.VanBan;
                if (_r.DictionaryPmStrings.TryGetValue(dh.CongTrinhId, out Data.DO.Business.StringDO? sct))
                    if (tkdon.DuAn != null) tkdon.DuAn += $" - {sct.VanBan}";
                    else tkdon.DuAn = sct.VanBan;
                if (_r.DictionaryPmStrings.TryGetValue(dh.HangMucId, out Data.DO.Business.StringDO? shm))
                    if (tkdon.DuAn != null) tkdon.DuAn += $" - {shm.VanBan}";
                    else tkdon.DuAn = shm.VanBan;
                _dictionaryDonHang[tkdon.Id] = tkdon;
            }

            return theodonhang;
        }

        /// <summary>
        /// Load các công thức sử dụng và lưu vào trong dictionaryCongThuc
        /// </summary>
        /// <param name="ctids">Danh sách id của các công thức được dùng</param>
        /// <returns></returns>
        private async Task ThongKeCongThuc(int[] ctids)
        {
            _uniqueMaTP.Clear();
            dictionaryCongThuc.Clear();

            if (ctids.Length <= 0) return;

            // Tìm các công thức được sử dụng
            var lstct = await _db.HT_Phieu_CongThuc_SelectAsync($"id IN ({string.Join(",", ctids)})", null);
            foreach (var ct in lstct)
            {
                dictionaryCongThuc[ct.Id] = new TKCongThucDO(ct);
            }
            // Tìm các thành phần được sử dụng
            var lstcttp = await _db.ThongKe_CongThuc_ThanhPhan_Rel_Async(ctids.ToArray());

            HashSet<string> hashcttp = [];
            HashSet<int> tpids = [];
            List<TKCtTpDO> uniqueLstCtTp = [];
            foreach (var cttp in lstcttp)
            {
                string scttp = $"{cttp.CTId}-{cttp.TPId}";
                if (hashcttp.Contains(scttp)) continue;
                hashcttp.Add(scttp);
                tpids.Add(cttp.TPId);
                uniqueLstCtTp.Add(cttp);
            }

            var lsttp = await _db.ThongKe_CongThuc_ThanhPhan_Async(tpids.ToArray());
            Dictionary<int, DHThanhPhanDO> dicThanhPhan = [];
            string? lastma = null;
            foreach (var tp in lsttp)
            {
                if (tp.NL_Ma != null && (lastma == null || tp.NL_Ma != lastma))
                {
                    lastma = tp.NL_Ma;
                    _uniqueMaTP.Add(new TKThanhPhanDO(lastma, tp.NL_Ten, (Core.LoaiThanhPhan)tp.NL_PhanLoai, tp.NL_Silo, tp.RoundDigit));
                }
                dicThanhPhan[tp.Id] = tp;
            }
            foreach (var cttp in uniqueLstCtTp)
            {
                if (dictionaryCongThuc.ContainsKey(cttp.CTId))
                {
                    dictionaryCongThuc[cttp.CTId].DsThanhPhan.Add(dicThanhPhan[cttp.TPId]);
                }
            }
        }

        /// <summary>
        /// Thêm các cột thành phần vào bảng
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="uniqueMaTP"></param>
        private void CreateDataGridColumns(DataGrid dg, List<TKThanhPhanDO> uniqueMaTP)
        {
            // TODO: tối ưu xóa cột?
            DataGridTextColumn col;
            if (dg.Columns.Count > 10)
            {
                while (dg.Columns.Count > 10)
                    dg.Columns.RemoveAt(dg.Columns.Count - 1);
            }

            for (int i = 0; i < uniqueMaTP.Count; i++)
            {
                var headerStyle = new Style(typeof(DataGridColumnHeader));
                if (uniqueMaTP[i].PhanLoai == Core.LoaiThanhPhan.CotLieu)
                    headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Blue));
                else if (uniqueMaTP[i].PhanLoai == Core.LoaiThanhPhan.XiMang)
                    headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Goldenrod));
                else if (uniqueMaTP[i].PhanLoai == Core.LoaiThanhPhan.PhuGia)
                    headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.OrangeRed));
                else if (uniqueMaTP[i].PhanLoai == Core.LoaiThanhPhan.Nuoc)
                    headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.SkyBlue));
                col = new DataGridTextColumn
                {
                    Header = uniqueMaTP[i].Ten,
                    HeaderStyle = headerStyle,
                    Binding = new Binding($"TP{i}")
                };
                dg.Columns.Add(col);
            }

            col = new DataGridTextColumn
            {
                Header = "Trạng thái",
                Binding = new Binding("TrangThai")
            };
            dg.Columns.Add(col);
        }

        /// <summary>
        /// Tính trạng thái của mẻ từ flags của mẻ
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public string GetTTFromFlags(int flags)
        {
            return ((flags & 1) == 1 ? "Tự động" : "") + ((flags & 2) == 2 ? "Dừng" : "") + ((flags & 4) == 4 ? " | Mô phỏng" : "");
        }

        private void BtExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new()
            {
                Filter = "Excel files|*.xlsx",
                FileName = $"BaoCao_{DateTime.Now:yyyyMMddHHmm}"
            };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    ExcelExporter.ExportDataGridVisibleText(Dgrid1, sfd.FileName);
                    GlobalUIEvent.Instance.RaiseEvent(this, GlobalUIEventKinds.DebugMsg, $"Xuất thống kê ra: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    GlobalUIEvent.Instance.RaiseEvent(this, GlobalUIEventKinds.DebugMsg, $"Lỗi xuất excel: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Tách số phiếu từ chuỗi fitler.
        /// Ví dụ: 1, 2 -> 1,2
        /// 1-5 -> 1,2,3,4,5
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string PhanTichSoPhieu(string filter)
        {
            HashSet<int> sophieu = [];
            string[] ss = filter.Split(',');
            foreach (string s in ss)
            {
                int k = s.IndexOf('-');
                if (k < 0)
                {
                    if (int.TryParse(s, out int sp)) 
                        sophieu.Add(sp);
                }
                else if (k > 0)
                {
                    if (int.TryParse(s.Substring(0, k), out int sp1) && int.TryParse(s.Substring(k+1), out int sp2))
                    {
                        for (int j = sp1; j <= sp2; j++) sophieu.Add(j);
                    }
                }
            }

            List<string> ssp = [];
            foreach (int i in sophieu) ssp.Add($"'{i}'");

            return string.Join(",", ssp);
        }

        #region Thay đổi hiển thị
        private void ChkCapPhoi_Checked(object sender, RoutedEventArgs e)
        {
            ShowKQMe(ChkChiTietMe.IsChecked == true, ChkCapPhoi.IsChecked == true);
        }

        private void ChkCapPhoi_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowKQMe(ChkChiTietMe.IsChecked == true, ChkCapPhoi.IsChecked == true);
        }

        private void ChkChiTietMe_Checked(object sender, RoutedEventArgs e)
        {
            ShowKQMe(ChkChiTietMe.IsChecked == true, ChkCapPhoi.IsChecked == true);
        }

        private void ChkChiTietMe_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowKQMe(ChkChiTietMe.IsChecked == true, ChkCapPhoi.IsChecked == true);
        }
        #endregion

        private void BtExportXML_Click(object sender, RoutedEventArgs e)
        {
            var data = CreatePrintData();

            System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(typeof(List<TKMeInDO>));
            using (StreamWriter writer = File.CreateText(_r.ReportsPath + $"baocao{DateTime.Now:yyyyMMddHHmmss}.xml"))
            {
                x1.Serialize(writer, data);
            }
        }

        #region INotifyPropertyChanged
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = [];
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (_argsCache != null && propertyName != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChanged(_argsCache[propertyName]);
            }
        }

        private void NotifyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        #endregion

        private bool _is_printing = false;
        private async void BtPrint_Click(object sender, RoutedEventArgs e)
        {
            if (_is_printing) return;

            var dktk = new DKThongKeDO()
            {
                MaDon = TxtMaDon.Text,
                KhachHang = CboKH.Text,
                CongTrinh = TxtHangMuc.Text,
                SoPhieu = TxtSoPhieu.Text,
                BienSoXe = TxtBSX.Text,
                LaiXe = TxtLaiXe.Text,
                TGBD = DtStart.Value.ToString("dd/MM/yyyy HH:mm"),
                TGKT = DtEnd.Value.ToString("dd/MM/yyyy HH:mm"),
                MeTuDong = OptGoodBatch.IsChecked == true,
                MeDungTay = OptGoodBatch.IsChecked == true,
                MeMoPhong = ChkSimBatch.IsChecked == true,
            };

            _is_printing = true;
            await _inbaocao.Print(dktk, KQMe.ToList(), _uniqueMaTP, ChkInChiTiet.IsChecked == true);
            _is_printing = false;
        }

        private List<TKMeInDO> CreatePrintData()
        {
            List<TKMeInDO> data = [];
            TKMeInDO maThanhPhan = new()
            {
                DonId = -1,
                PhieuId = -1,
                MeId = -1,
                STT = 0,
                Flags = 64,
            };
            for (int i = 0; i < 15 && i < _uniqueMaTP.Count; i++)
            {
                maThanhPhan.SetTP(i, _uniqueMaTP[i].Ten);
            }
            data.Add(maThanhPhan);

            foreach (var me in KQMe)
            {                
                data.Add(me);
            }

            return data;
        }
    }
}
