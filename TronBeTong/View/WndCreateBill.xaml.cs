using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndCreateBill.xaml.
    /// KHÔNG DÙNG DO KHÔNG PHÙ HỢP VỚI LOGIC TRẠM TRỘN
    /// </summary>
    public partial class WndCreateBill : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = new();

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

        private DbRepository _r = DbRepository.Instance;

        public ObservableCollection<KDKhachHangVM> DsKH { get; set; }
        public ObservableCollection<KDDuAnVM> DsDuAn { get; set; }

        public ObservableCollection<KDXeVM> DsXe { get; set; }

        public ObservableCollection<KDLaiXeVM> DsLaiXe { get; set; }

        public ObservableCollection<BTCongThucVM> DsCongThuc { get; set; }
        
        public DHDonVM Don { get; set; } = new DHDonVM();

        public WndCreateBill()
        {
            InitializeComponent();
            DataContext = this;

            DsKH = _r.DsKH;
            DsDuAn = _r.DsDA;
            DsXe = _r.DsXe;
            DsLaiXe = _r.DsLaiXe;
            DsCongThuc = _r.DsCongThuc;

            CboXe.ZNextFocus = CboLaiXe.InnerTextBox;
            CboLaiXe.ZNextFocus = CboCongThuc.InnerTextBox;
            CboCongThuc.ZNextFocus = TxtSlump;
        }

        #region Comboboxes
        private void CboKH_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is KDKhachHangVM kh)
                {
                    CboKH.ZText = kh.Ten;
                    Don.KhachHang = kh;
                }
            }
        }

        private void CboDA_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is KDDuAnVM da)
                {
                    CboDA.ZText = da.DuAn;
                    Don.DuAn = da;
                }
            }
        }

        private void CboXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
        }

        private void CboLaiXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
        }

        private void CboCongThuc_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is BTCongThucVM ct)
                {
                    //Don.CongThuc = new DHCongThucVM();
                    //Don.CongThuc.CopyFrom(ct);
                    //CboCongThuc.ZText = Don.CongThuc.Ma;
                    //await _r.CongThuc_LoadThanhPhan(Don.CongThuc);
                    //Don.CongThuc.Id = -1;                               // Xóa Id vì sẽ lưu ở bảng khác với BTCongThucDO
                    //SiloDsCauHinh.Instance.CheckCongThuc(Don.CongThuc);
                    //Don.CongThuc.ApplyM3(TxtM3.Value);
                }
            }
        }
        #endregion

        private void BtCreate_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(CboXe.ZText))
            //{
            //    if (Don.Xe == null || Don.Xe.BSX != CboXe.ZText) {                    
            //        Don.Xe = DsXe.FirstOrDefault(x => x.BSX == CboXe.ZText);
            //        if (Don.Xe == null) CboXe.ZText = null;
            //    }
            //}
            //if (!string.IsNullOrEmpty(CboLaiXe.ZText))
            //{
            //    if (Don.LaiXe == null || Don.LaiXe.Ten != CboLaiXe.ZText)
            //    {
            //        Don.LaiXe = DsLaiXe.FirstOrDefault(x => x.Ten == CboLaiXe.ZText);
            //        if (Don.LaiXe == null) CboLaiXe.ZText = null;
            //    }
            //}

            //await _r.DonHang_Save(Don);
        }

        private void TxtM3_ValueChanged(object sender, double e)
        {
            //Don.TheTichDat = TxtM3.Value;
            //Don.CongThuc?.ApplyM3(TxtM3.Value);
        }

        private void BtAutoCalSoMe_Click(object sender, RoutedEventArgs e)
        {
            //if (Don.Xe != null && Don.Xe.DungTich > 0)
            //{
                //double me = Don.TheTichDat / Don.Xe.DungTich;
                //int nme = (int)me;
                //if (me > nme) nme++;
                //TxtSoMe.Value = nme;
            //}
        }

        private void TxtSoMe_ValueChanged(object sender, int e)
        {
            //Don.MeDat = e;
            //Don.CongThuc?.ApplySoMe(Don.MeDat);
            //if (e > 0)
            //{
            //    TxtM3OverMe.Text = Math.Round(Don.TheTichDat / e, 3).ToString();
            //}
        }
    }
}
