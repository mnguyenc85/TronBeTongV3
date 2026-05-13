using NMWPFControls.Core.MVVM;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Reports;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndEditKhachHang.xaml
    /// </summary>
    public partial class WndEditKhachHang : Window, INotifyPropertyChanged
    {
        private DbRepository _r = DbRepository.Instance;

        public KDKhachHangVM CurKH { get; set; } = new KDKhachHangVM();
        public KDDsKhachHangVM Ds { get; set; } = new();
        public RelayCommand CmdSave { get; private set; }

        public bool Selected { get; set; }
        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndEditKhachHang()
        {
            InitializeComponent();
            DataContext = this;

            CmdSave = new RelayCommand(HandleSave, CanSave);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ds.Filter.Run(null, true);
            LvKhachHang.SelectedItems.Clear();
            SetTitle();

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private async void HandleSave(object obj)
        {
            if (string.IsNullOrEmpty(CurKH.Ma) || string.IsNullOrEmpty(CurKH.Ten))
            {
                MessageBox.Show("Mã và Tên không để trống!", "Lưu khách hàng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                CurKH.Updated = DateTime.Now;
                await Ds.Save(CurKH);
                SetTitle();
            }
        }
        private bool CanSave(object obj)
        {
            return CurKH.IsChanged;
        }
        private async void CheckSave()
        {
            if (CurKH.IsChanged)
            {
                if (MessageBox.Show("Bạn có muốn lưu thông tin hiện tại?", "Lưu khách hàng", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await Ds.Save(CurKH);
                }
            }
        }

        private void BtNew_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            CurKH.Reset();
            LvKhachHang.SelectedItem = null;
            SetTitle();
        }

        private void BtClone_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            CurKH.Id = -1;
            CurKH.IsChanged = false;
            _skipLvChanged = true;
            LvKhachHang.SelectedItem = null;
            _skipLvChanged = false;
            SetTitle();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            Ds.Filter.Run(TxtFilter.Text);
        }

        private bool _skipLvChanged = false;
        private void LvKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_skipLvChanged) return;
            if (LvKhachHang.SelectedItem is KDKhachHangVM k)
            {
                CurKH.CopyFrom(k);
            }
            else
            {
                CurKH.Reset();
            }
            CurKH.IsChanged = false;
            SetTitle();
        }

        private void SetTitle()
        {
            if (CurKH.Id > 0)
            {
                LblKHTitle.Text = $"Sửa thông tin: {CurKH.Ma}";
            }
            else
            {
                LblKHTitle.Text = $"Tạo thông tin mới";
            }
        }

        #region Import & Export
        private void BtExportCsv_Click(object sender, RoutedEventArgs e)
        {
            Ds.ExportCsv();
        }

        private async void BtImportCsv_Click(object sender, RoutedEventArgs e)
        {
            await Ds.ImportCsv();
        }
        #endregion

        public void EnableBtSelect(bool enable)
        {
            BtSelect.IsEnabled = enable;
        }

        private void BtSelect_Click(object sender, RoutedEventArgs e)
        {
            Selected = true;
            Close();
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
    }
}
