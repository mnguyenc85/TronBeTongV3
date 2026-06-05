using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TronBeTongV3.Comm;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel.DonHang;
using TronBeTongV3.Data.ViewModel.ThongKe;
using TronBeTongV3.Reports;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndViewDonHang.xaml
    /// </summary>
    public partial class WndViewDonHang : Window, INotifyPropertyChanged
    {
        private DbBridge _db = DbBridge.Instance;
        private DbRepository _r = DbRepository.Instance;
        // Tạo CultureInfo dựa trên "en-US" nhưng đổi dấu thập phân thành ','
        private CultureInfo _vnCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public DHDonVM DonHang { get; set; } = new DHDonVM();

        #region Danh sách phieu
        public ICollectionView CVDsPhieu { get; set; }
        public DHPhieuVM? SelectedPhieu { get; set; }
        #endregion

        private DateTime filterTime1;
        private DateTime filterTime2;

        private TKMeInVM? Silo_MaNL;
        private TKMeInVM? Silo_TongKL;

        public ObservableCollection<TKMeInVM> DsMe { get; set; } = [];

        private XuLyInPhieu _inphieu = XuLyInPhieu.Instance;

        public WndViewDonHang()
        {
            InitializeComponent();
            DataContext = this;

            CVDsPhieu = CollectionViewSource.GetDefaultView(DonHang.DsPhieu);
            CVDsPhieu.SortDescriptions.Add(new SortDescription("TGBD", ListSortDirection.Descending));
            TxtPrintRepeat.Value = (int)_r.Settings.GetDoubleValue("in.phieu.auto.times", 1);

            _vnCulture.NumberFormat.NumberDecimalSeparator = ",";
            _vnCulture.NumberFormat.NumberGroupSeparator = "."; // nếu muốn ngăn cách hàng nghìn

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DonHang != null)
            {
                await _r.DonHang_LoadAllPhieu(DonHang);
            }

            if (LvMe.View is GridView gv)
            {
                for (int i = 0; i < ModelHeThong.SoCLReal; i++)
                {
                    gv.Columns.Add(new GridViewColumn()
                    {
                        Header = $"CL{i + 1} ",
                        DisplayMemberBinding = new Binding($"KLCL{i}")
                    });
                }
                for (int i = 0; i < ModelHeThong.SoXMReal; i++)
                {
                    gv.Columns.Add(new GridViewColumn()
                    {
                        Header = $"Xi{i + 1} ",
                        DisplayMemberBinding = new Binding($"KLXi{i}")
                    });
                }
                for (int i = 0; i < ModelHeThong.SoPGReal; i++)
                {
                    gv.Columns.Add(new GridViewColumn()
                    {
                        Header = $"PG{i + 1} ",
                        DisplayMemberBinding = new Binding($"KLPG{i}")
                    });
                }
                gv.Columns.Add(new GridViewColumn()
                {
                    Header = "Nước ",
                    DisplayMemberBinding = new Binding("KLNuoc")
                });
                gv.Columns.Add(new GridViewColumn()
                {
                    Header = "Hoàn thành ",
                    DisplayMemberBinding = new Binding("HoanThanh")
                });
            }

            bool phieuAllowGhiM3 = _r.Settings.GetBoolValue("hack.phieu.m3");
            PnlHack.Visibility = phieuAllowGhiM3? Visibility.Visible : Visibility.Collapsed;

            if (_inphieu.ReportTemplate == null) _inphieu.LoadTemplate();

            double colw = _r.Settings.GetDoubleValue("app.wndviewdonhang.grid.col2", 300);
            ColDef2.Width = new GridLength(colw);
        }
        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            _r.Settings.UpdateDouble("in.phieu.auto.times", TxtPrintRepeat.Value);
            _r.Settings.UpdateDouble("app.wndviewdonhang.grid.col2", ColDef2.ActualWidth);
            await _db.SaveSettingsAsync(_r.Settings);
        }

        private async void LvPhieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading_me) { e.Handled = true; return; }
            if (SelectedPhieu != null)
            {
                await LoadMe(SelectedPhieu.Id);
                TxtHackSoPhieu.Text = SelectedPhieu.STT.ToString();
                TxtHackPhieuNgay.Text = SelectedPhieu.TGBD?.ToString("dd/MM/yyyy");
                
                TxtHackKhachHang.Text = DonHang.KhachHang?.Ten;
                TxtHackCongTrinh.Text = DonHang.DuAn?.DuAn;
                TxtHackCongTrinhDiaChi.Text = DonHang.DuAn?.DiaChi;

                TxtHackBienSoXe.Text = SelectedPhieu.Xe?.BSX;
                TxtHackLaiXe.Text = SelectedPhieu.LaiXe?.Ten;

                TxtHackBeTongMac.Text = SelectedPhieu.CongThuc?.Mac;
                TxtHackBeTongSlump.Text = SelectedPhieu.CongThuc?.Slump;
                TxtHackBeTongAggMax.Text = SelectedPhieu.CongThuc?.KTHat.ToString("F0");
                
                TxtHackCode.Text = "";

                TxtHackTGRoiTram.Text = SelectedPhieu.TGHT?.ToString("HH:mm");

                TxtHackPhieuM3Tron.Text = string.Format(_vnCulture, "{0:F2}", SelectedPhieu.TheTichHT);
                TxtHackPhieuTichLuy.Text = string.Format(_vnCulture, "{0:F1}", SelectedPhieu.TheTichTichLuy);
                TxtSeal.Text = SelectedPhieu.KepChi;
            }
            else
            {
                TxtHackSoPhieu.Text = "";
                TxtHackPhieuNgay.Text = "";
                TxtHackKhachHang.Text = "";
                TxtHackCongTrinh.Text = "";
                TxtHackCongTrinhDiaChi.Text = "";
                TxtHackBienSoXe.Text = "";
                TxtHackLaiXe.Text = "";
                TxtHackBeTongMac.Text = "";
                TxtHackBeTongSlump.Text = "";
                TxtHackBeTongAggMax.Text = "";
                TxtHackCode.Text = "";
                TxtHackTGRoiTram.Text = "";
                TxtHackPhieuM3Tron.Text = "";
                TxtHackPhieuTichLuy.Text = "";
            }
        }

        private bool _loading_me = false;
        private async Task LoadMe(int pid)
        {
            DsMe.Clear();
            if (SelectedPhieu == null) return;

            _loading_me = true;
            var conn = await _db.OpenConnAsync();
            var lst = await _db.HT_Me_LoadByPhieu(conn, pid, true);
            await _db.CloseConnAsync(conn);

            if (SelectedPhieu.CongThuc != null)
            {
                // Mã thành phần
                Silo_MaNL = new() { Flags = 64 };
                Silo_MaNL.TenNLFromDHCongThuc(SelectedPhieu.CongThuc);
                DsMe.Add(Silo_MaNL);

                // Cấp phối chuẩn
                TKMeInVM Silo_CPChuan = new() { Flags = 256 };
                Silo_CPChuan.CapPhoiFromDHCongThuc(SelectedPhieu.CongThuc, "CP Chuẩn");
                DsMe.Add(Silo_CPChuan);

                // Cấp phối mẻ
                TKMeInVM Silo_CPMe = new() { Flags = 512 };
                Silo_CPMe.CapPhoiFromDHCongThuc(SelectedPhieu.CongThuc, $"CP Mẻ", SelectedPhieu.TheTichMe);
                DsMe.Add(Silo_CPMe);
            }

            // KL từng mẻ
            int n = 0;
            TKMeInVM me;
            foreach (var i in lst)
            {
                me = new()
                {
                    STT = (++n).ToString(),
                    Flags = i.Flags,
                };
                me.FromDHMeDO(i);
                DsMe.Add(me);
            }

            // Tổng KL
            SelectedPhieu.KiemTraThongTin(lst);
            Silo_TongKL = new() { Flags = 128 };
            Silo_TongKL.FromPhieuTongKL(SelectedPhieu);
            DsMe.Add(Silo_TongKL);

            _loading_me = false;
        }

        private async void BtInPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhieu != null && Silo_MaNL != null && Silo_TongKL != null)
            {
                if (SelectedPhieu.KepChi != TxtSeal.Text)
                {
                    SelectedPhieu.KepChi = TxtSeal.Text;
                    await DbBridge.Instance.HT_Phieu_UpdateUserInfoAsync(SelectedPhieu.CreateDO());
                }

                HackValue? ghiDe = null;
                if (PnlHack.Visibility == Visibility.Visible)
                {
                    ghiDe = new HackValue()
                    {
                        SoPhieu = TxtHackSoPhieu.Text,
                        NgayTron = TxtHackPhieuNgay.Text,
                        KhachHang = TxtHackKhachHang.Text,
                        DuAn = TxtHackCongTrinh.Text,
                        DuAnDiaChi = TxtHackCongTrinhDiaChi.Text,
                        BienSoXe = TxtHackBienSoXe.Text,
                        LaiXe = TxtHackLaiXe.Text,
                        BeTongMac = TxtHackBeTongMac.Text,
                        BeTongSlump = TxtHackBeTongSlump.Text,
                        BeTongMaxAgg = TxtHackBeTongAggMax.Text,
                        MaSo = TxtHackCode.Text,
                        TGRoiTram = TxtHackTGRoiTram.Text,
                        M3 = TxtHackPhieuM3Tron.Text,
                        TichLuy = TxtHackPhieuTichLuy.Text,
                    };
                }
                int solan = TxtPrintRepeat.Value; if (solan < 1) { solan = 1; TxtPrintRepeat.Text = "1"; }

                if (await _inphieu.Print(SelectedPhieu, DsMe.ToList(), ChkInChiTiet.IsChecked == true, ghiDe, false, solan))
                {
                    GlobalUIEvent.Instance.RaiseEvent(this, GlobalUIEventKinds.DebugMsg, $"In phiếu {SelectedPhieu.SoPhieu}");
                }
            }
        }

        private void BtPhieuXml_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhieu != null)
            {
                System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(SelectedPhieu.GetType());
                using (StreamWriter writer = File.CreateText(_r.ReportsPath + $"phieu{SelectedPhieu.SoPhieu:000}.xml"))
                {
                    x1.Serialize(writer, SelectedPhieu);
                }

                System.Xml.Serialization.XmlSerializer x2 = new System.Xml.Serialization.XmlSerializer(typeof(List<TKMeInVM>));
                using (StreamWriter writer = File.CreateText(_r.ReportsPath + $"phieu{SelectedPhieu.SoPhieu:000}_dsme.xml"))
                {
                    x2.Serialize(writer, DsMe.ToList());
                }
            }

        }

        #region Lọc phiếu
        private void ChkFilterDate_Unchecked(object sender, RoutedEventArgs e)
        {
            LocPhieu();
        }

        private void TxtFilterBSX_TextChanged(object sender, TextChangedEventArgs e)
        {
            LocPhieu();
        }

        private void TxtFilterLaiXe_TextChanged(object sender, TextChangedEventArgs e)
        {
            LocPhieu();
        }

        private void ChkFilterDate_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                filterTime1 = DtFilterDate.Value;
                filterTime2 = DtFilterDate.Value + new TimeSpan(24, 0, 0);
                LocPhieu();
            }
            catch { }
        }

        private void DtFilterDate_ValueChanged(object sender, DateTime e)
        {
            try
            {
                filterTime1 = DtFilterDate.Value;
                filterTime2 = DtFilterDate.Value + new TimeSpan(24, 0, 0);
                LocPhieu();
            }
            catch { }
        }

        private void LocPhieu()
        {
            CVDsPhieu.Filter = p =>
            {
                if (p is DHPhieuVM ph)
                {
                    if (ChkFilterDate.IsChecked == true && (ph.TGHT < filterTime1 || ph.TGHT > filterTime2))
                        return false;
                    if (!string.IsNullOrEmpty(TxtFilterBSX.Text) && (ph.Xe == null || ph.Xe.BSX == null || !ph.Xe.BSX.Contains(TxtFilterBSX.Text, StringComparison.CurrentCultureIgnoreCase))) return false;
                    if (!string.IsNullOrEmpty(TxtFilterLaiXe.Text) && (ph.LaiXe == null || ph.LaiXe.Ten == null || !ph.LaiXe.Ten.Contains(TxtFilterLaiXe.Text, StringComparison.CurrentCultureIgnoreCase))) return false;
                    return true;
                }
                return false;
            };
        }

        #endregion

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
    }
}
