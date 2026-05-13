using NMWPFControls.Core.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndDuAn.xaml
    /// </summary>
    public partial class WndEditDuAn : Window, INotifyPropertyChanged
    {
        private DbRepository _r = DbRepository.Instance;
        public KDDuAnVM CurDA { get; set; } = new KDDuAnVM();
        public KDDsDuAnVM Ds { get; set; } = new KDDsDuAnVM();
        public RelayCommand CmdSave { get; private set; }

        public ObservableCollection<KDKhachHangVM> DsKH { get; set; }

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndEditDuAn()
        {
            InitializeComponent();
            DataContext = this;

            DsKH = _r.DsKH;
            CboKH.SortDescriptions.Add(new System.ComponentModel.SortDescription("Updated", System.ComponentModel.ListSortDirection.Descending));

            CmdSave = new RelayCommand(HandleSave, CanSave);

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ds.Filter.Run(null, true);

            CboTenDuAn.SelectedIndex = -1;
            CboTenCongTrinh.SelectedIndex = -1;
            CboTenHangMuc.SelectedIndex = -1;
            CboTenDiaChi.SelectedIndex = -1;
            GrdDuAn.SelectedItems.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void HandleSave(object obj)
        {
            if (string.IsNullOrEmpty(CurDA.DuAn))
            {
                MessageBox.Show("Dự án không để trống!", "Lưu dự án", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                await Ds.Save(CurDA);
                SetTitle();
            }
        }
        private bool CanSave(object obj)
        {
            return CurDA.IsChanged;
        }
        private async void CheckSave()
        {
            if (CurDA.IsChanged)
            {
                if (MessageBox.Show("Bạn có muốn lưu thông tin hiện tại?", "Lưu dự án", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await Ds.Save(CurDA);
                }
            }
        }

        private void BtNew_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            CurDA.Reset();
            GrdDuAn.SelectedItems.Clear();
            SetTitle();
            CboKH.ZText = "";
        }

        private void GrdDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GrdDuAn.SelectedItem is KDDuAnVM d)
            {
                CurDA.CopyFrom(d);
                var kh = DsKH.FirstOrDefault(x => x.Id == d.KHId);
                CboKH.ZSelectedItem = kh;
                CboKH.ZText = kh?.Ten;
            }
            else
            {
                CurDA.Reset();               
            }
            CurDA.IsChanged = false;
            SetTitle();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            Ds.Filter.Run(TxtFilter.Text);
        }

        private void BtClone_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            CurDA.Id = -1;
            CurDA.IsChanged = false;
            SetTitle();
        }

        private void SetTitle()
        {
            if (CurDA.Id > 0)
            {
                LblDATitle.Text = $"Sửa thông tin: {CurDA.STT}";
            }
            else
            {
                LblDATitle.Text = $"Tạo thông tin mới";
            }
        }

        private async void BtImportCsv_Click(object sender, RoutedEventArgs e)
        {
            await Ds.ImportCsv(DsKH);
        }

        private void BtExportCsv_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaSaveFileDialog sfd = new()
            {
                Filter = "CSV files|*.csv"
            };
            if (sfd.ShowDialog() == true)
            {
                Ds.ExportCsv(sfd.FileName, DsKH);
            }
        }

        private void CboKH_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is KDKhachHangVM kh)
                {
                    CurDA.KHId = kh.Id;
                    CurDA.KHMa = kh.Ten;
                    CboKH.ZText = kh.Ten;
                }
                else
                {
                    CurDA.KHId = -1;
                    CurDA.KHMa = null;
                }
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

    }
}
