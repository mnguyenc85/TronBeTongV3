using NMWPFControls.Core;
using NMWPFControls.Core.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndEditSilo.xaml
    /// </summary>
    public partial class WndEditSilo : Window, INotifyPropertyChanged
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

        private DbBridge _db = DbBridge.Instance;

        public List<SiloCauHinhVM> DsSilos { get; set; }
        // Thay bằng SiloCauHinhVM?
        public LoaiThanhPhan LoaiSilo { get; set; } = LoaiThanhPhan.CotLieu;
        public int SiloIndex { get; set; }

        #region Nguyen lieu
        private DelayUpdate<string> _filterSiloNL;
        private int _stt = 0;
        public ObservableCollection<SiloNguyenLieuVM> DsNL { get; set; }
        private CollectionViewSource _cvsDsNL = new();
        public ICollectionView CVDsNL { get { return _cvsDsNL.View; } }

        public SiloNguyenLieuVM CurNL { get; set; } = new() { PhanLoai = LoaiThanhPhan.CotLieu, IsChanged = false };
        public RelayCommand CmdNLSave { get; private set; }
        #endregion

        public CtrlTPCotLieu2? TPCotLieu { get; set; }
        public CtrlTPPhuGia? TPPhuGia { get; set; }
        public CtrlTPXiMang? TPXiMang { get; set; }

        public WndEditSilo()
        {
            InitializeComponent();
            DataContext = this;

            DsSilos = CauHinhTramTron.Instance.DsSilos;

            DsNL = DbRepository.Instance.DsNguyenLieu;
            _cvsDsNL.Source = DsNL;
            _filterSiloNL = new(300) {
                ExecUpdate = f => {
                    _stt = 0;
                    CVDsNL.Filter = x =>
                    {
                        if (x is SiloNguyenLieuVM c)
                        {
                            if (c.PhanLoai == LoaiSilo)
                            {
                                if (string.IsNullOrEmpty(f) || (c.Ten != null && c.Ten.Contains(f, StringComparison.CurrentCultureIgnoreCase))
                                    || (c.Ma != null && c.Ma.Contains(f, StringComparison.CurrentCultureIgnoreCase))) { 
                                    c.STT = ++_stt;
                                    return true; 
                                }
                            }
                        }
                        return false;
                    };
                }
            };
            CmdNLSave = new(ExecNLSave, CanNLSave);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _filterSiloNL.Run(null, true);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterSiloNL.Run(TxtFilter.Text);
        }
        private void BtFilterClear_Click(object sender, RoutedEventArgs e)
        {
            TxtFilter.Text = "";
        }

        private void LvNguyenLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvNguyenLieu.SelectedItem is SiloNguyenLieuVM ch)
            {
                CurNL.CopyFrom(ch);
                CurNL.IsChanged = false;
                SetTitles();
            }
        }

        public void SelectSilo(LoaiThanhPhan loai, int index)
        {
            var silo = DsSilos.FirstOrDefault(si => si.PhanLoai == loai && si.Index == index);
            LvSilos.SelectedItem = silo;
            if (silo == null)
            {
                LoaiSilo = loai;
            }
        }

        private void LvSilos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvSilos.SelectedItem is SiloCauHinhVM ch)
            {
                LblSilo.Text = ch.Ten;
                SiloIndex = ch.Index;
                LoaiSilo = ch.PhanLoai;

                _filterSiloNL.Run(TxtFilter.Text, true);

                if (CurNL.PhanLoai != LoaiSilo)
                {
                    CurNL.Reset();
                    CurNL.PhanLoai = LoaiSilo;
                    CurNL.IsChanged = false;
                }

                BdrNguyenLieuInputs.IsEnabled = ch.PhanLoai != LoaiThanhPhan.Nuoc;

                SetTitles();
            }
        }

        private bool CanNLSave(object obj)
        {
            return CurNL.IsChanged;
        }

        private async void ExecNLSave(object obj)
        {
            await SaveNL();
        }

        private void BtNLNew_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            CurNL.Reset();
            CurNL.PhanLoai = LoaiSilo;
            CurNL.IsChanged = false;
            SetTitles();
        }

        private async Task SaveNL()
        {
            if (string.IsNullOrEmpty(CurNL.Ma))
            {
                MessageBox.Show("Mã nguyên liệu không để trống", "Lưu nguyên liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CurNL.IsChanged) return;

            bool succ = await DbRepository.Instance.NguyenLieu_Save(CurNL);
            if (!succ)
            {
                MessageBox.Show("Lỗi lưu nguyên liệu:\r\n- Mã đã tồn tại!", "Lưu nguyên liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetTitles();
        }

        private async void CheckSave()
        {
            if (CurNL.IsChanged)
            {
                if (MessageBox.Show("Bạn có muốn lưu thông tin hiện tại?", "Nguyên liệu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await SaveNL();
                }
            }
        }

        private void SetTitles()
        {
            if (CurNL.Id > 0)
                LblNLTitle.Text = $"Sửa: {CurNL.Ma}";
            else
                LblNLTitle.Text = $"Tạo nguyên liệu mới";
        }

        private async void BtNLSet_Click(object sender, RoutedEventArgs e)
        {
            await SaveNL();
            if (CurNL.Id > 0 && CurNL.PhanLoai == LoaiSilo)
            {
                await CauHinhTramTron.Instance.SetNguyenLieu(LoaiSilo, SiloIndex, CurNL);
                if (LoaiSilo == LoaiThanhPhan.CotLieu)
                    TPCotLieu?.SetupBin(SiloIndex);
                else if (LoaiSilo == LoaiThanhPhan.XiMang)
                    TPXiMang?.SetupBin(SiloIndex);
                else if (LoaiSilo == LoaiThanhPhan.PhuGia)
                    TPPhuGia?.SetupBin(SiloIndex);
            }
        }

        private async void BtNLClear_Click(object sender, RoutedEventArgs e)
        {
            await CauHinhTramTron.Instance.SetNguyenLieu(LoaiSilo, SiloIndex, null);
            if (LoaiSilo == LoaiThanhPhan.CotLieu)
                TPCotLieu?.SetupBin(SiloIndex);
            else if (LoaiSilo == LoaiThanhPhan.XiMang)
                TPXiMang?.SetupBin(SiloIndex);
            else if (LoaiSilo == LoaiThanhPhan.PhuGia)
                TPPhuGia?.SetupBin(SiloIndex);
        }
    }
}
