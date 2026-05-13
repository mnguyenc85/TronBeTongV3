using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndCreateOrder.xaml
    /// </summary>
    public partial class WndCreateOrder : Window
    {
        private DbRepository _r = DbRepository.Instance;

        public DHDonVM DonHang { get; set; } = new() { DuAn = new KDDuAnVM() };
        public ObservableCollection<KDKhachHangVM> DsKH { get; set; }

        private List<KDDuAnVM> _foundedDuAn = new();
        public ObservableCollection<string> DsDuAn { get; set; } = [];
        public ObservableCollection<string> DsCongTrinh { get; set; } = [];
        public ObservableCollection<string> DsHangMuc { get; set; } = [];

        public WndCreateOrder()
        {
            InitializeComponent();
            DataContext = this;

            DsKH = _r.DsKH;
            CboKH.SortDescriptions.Add(new SortDescription("Updated", ListSortDirection.Descending));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DsDuAn.Clear();
            DsCongTrinh.Clear();
            DsHangMuc.Clear();
        }

        #region ComboBoxes
        private void CboKH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Code cho ComboBox cũ
            //FilterDuAn(CboKH.Selected as KDKhachHangVM);
        }

        private void CboKH_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                var kh = e.Selected as KDKhachHangVM;
                if (kh != null)
                    CboKH.ZText = kh.Ten;
                DonHang.KhachHang = kh;
                FilterDuAn(kh);
            }
        }

        private void CboDA_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                CboDA.ZText = e.Selected as string;
                FilterCongTrinh();
            }
        }

        private void CboDA_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CboDA.ZText != CboDA.ZSelectedItem?.ToString())
                FilterCongTrinh();
        }

        /// <summary>
        /// Lọc dự án theo khách hàng
        /// </summary>
        /// <param name="kh"></param>
        private void FilterDuAn(KDKhachHangVM? kh)
        {
            DsDuAn.Clear();
            DsCongTrinh.Clear();
            _foundedDuAn.Clear();
            if (kh != null)
            {
                HashSet<string> tenDuAn = new HashSet<string>();
                HashSet<string> tenCongTrinh = new HashSet<string>();

                foreach (var da in _r.DsDA)
                {
                    if (da.KHId == kh.Id)
                    {
                        _foundedDuAn.Add(da);

                        if (da.DuAn != null && !tenDuAn.Contains(da.DuAn))
                        {
                            tenDuAn.Add(da.DuAn);
                            DsDuAn.Add(da.DuAn);
                        }
                        if (da.CongTrinh != null && !tenCongTrinh.Contains(da.CongTrinh))
                        {
                            tenCongTrinh.Add(da.CongTrinh);
                            DsCongTrinh.Add(da.CongTrinh);
                        }
                    }
                }
            }
        }

        private void FilterCongTrinh()
        {
            string? s = CboDA.ZText;

            DsCongTrinh.Clear();
            HashSet<string> keys = [];

            if (s == null)
            {
                foreach (var da in _foundedDuAn)
                {
                    if (da.CongTrinh != null && !keys.Contains(da.CongTrinh))
                    {
                        keys.Add(da.CongTrinh);
                        DsCongTrinh.Add(da.CongTrinh);
                    }
                }
            }
            else
            {
                foreach (var da in _foundedDuAn)
                {
                    if (da.DuAn == s && da.CongTrinh != null && !keys.Contains(da.CongTrinh))
                    {
                        keys.Add(da.CongTrinh);
                        DsCongTrinh.Add(da.CongTrinh);
                    }
                }
            }

            FindDiaChi();
        }

        private void CboCT_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                CboCT.ZText = e.Selected as string;
                FilterHangMuc();
            }
        }

        private void CboCT_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CboCT.ZText != CboCT.ZSelectedItem?.ToString())
                FilterHangMuc();
        }

        private void FilterHangMuc()
        {
            string? s = CboCT.ZText;

            DsHangMuc.Clear();
            HashSet<string> keys = new HashSet<string>();

            if (s == null)
            {
                foreach (var da in _foundedDuAn)
                {
                    if (da.HangMuc != null && !keys.Contains(da.HangMuc))
                    {
                        keys.Add(da.HangMuc);
                        DsHangMuc.Add(da.HangMuc);
                    }
                }
            }
            else
            {
                foreach (var da in _foundedDuAn)
                {
                    if (da.CongTrinh == s && da.HangMuc != null && !keys.Contains(da.HangMuc))
                    {
                        keys.Add(da.HangMuc);
                        DsHangMuc.Add(da.HangMuc);
                    }
                }
            }
            
            FindDiaChi();
        }

        private void CboHM_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                CboHM.ZText = e.Selected as string;
                FindDiaChi();
            }
        }

        private void CboHM_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CboHM.ZText != CboHM.ZSelectedItem?.ToString())
                FindDiaChi();
        }

        private void FindDiaChi()
        {
            foreach (var da in _foundedDuAn)
            {
                if (da.HangMuc == null && da.CongTrinh == null && CboDA.ZText != null && da.DuAn == CboDA.ZText)
                {
                    TxtDiaChi.Text = da.DiaChi;
                    break;
                }
                else if (da.HangMuc == null && CboCT.ZText != null && da.CongTrinh == CboCT.ZText)
                {
                    TxtDiaChi.Text = da.DiaChi;
                    break;
                }
                else if (CboHM.ZText != null && da.HangMuc == CboHM.ZText)
                {
                    TxtDiaChi.Text = da.DiaChi;
                    break;
                }
            }
        }
        #endregion

        private async void BtSave_Click(object sender, RoutedEventArgs e)
        {
            if (DonHang.KhachHang == null)
            {
                if (CboKH.ZText != null)
                {
                    // TODO: bật tạo khách hàng với tên định sẵn
                }
                else
                {
                }
                MessageBox.Show("Phải chọn khách hàng!", "Tạo đơn hàng", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (DonHang.KhachHang.Ten != CboKH.ZText)
                {
                    if (MessageBox.Show($"Tạo đơn với khách hàng: {DonHang.KhachHang.Ten}?", "Tạo đơn hàng", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    CboKH.ZText = DonHang.KhachHang.Ten;
                }
            }

            if (DonHang.DuAn == null)
            {
                MessageBox.Show("Phải chọn dự án!", "Tạo đơn hàng", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                var da = DonHang.DuAn;
                if (string.IsNullOrWhiteSpace(da.DuAn) && string.IsNullOrWhiteSpace(da.CongTrinh) && string.IsNullOrWhiteSpace(da.HangMuc))
                {
                    MessageBox.Show("Thông tin dự án không được để trống!", "Tạo đơn hàng", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            DonHang.DuAn.DiaChi = TxtDiaChi.Text;
            await _r.DonHang_Save(DonHang);
            DialogResult = true;
        }

        private void BtEditKhachHang_Click(object sender, RoutedEventArgs e)
        {
            WndEditKhachHang wnd = new();
            wnd.EnableBtSelect(true);
            wnd.ShowDialog();
            if (wnd.Selected && wnd.CurKH != null)
            {
                DonHang.KhachHang = wnd.CurKH;
                CboKH.ZText = wnd.CurKH.Ten;
                FilterDuAn(wnd.CurKH);
            }
        }

        private void BtEditDuAn_Click(object sender, RoutedEventArgs e)
        {
            WndEditDuAn wnd = new();
            wnd.ShowDialog();
        }

        private void CboKH_LostFocus(object sender, RoutedEventArgs e)
        {
            FilterDuAn(CboKH.ZSelectedItem as KDKhachHangVM);
        }
    }
}
