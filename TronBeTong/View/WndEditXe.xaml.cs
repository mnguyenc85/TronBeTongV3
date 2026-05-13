using NMWPFControls.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndEditXe.xaml
    /// </summary>
    public partial class WndEditXe : Window, INotifyPropertyChanged
    {
        private int _xeid, _lxid;

        private readonly KDXeVM _cxe = new();
        public KDXeVM CurXe { get { return _cxe; } }
        private readonly KDLaiXeVM _clx = new();
        public KDLaiXeVM CurLX { get { return _clx; } }

        public KDDsXeVM Ds { get; set; } = new();
        public RelayCommand CmdSave { get; private set; }

        public ObservableCollection<KDXeVM> DsXe { get; set; }
        public ObservableCollection<KDLaiXeVM> DsLaiXe { get; set; }

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndEditXe()
        {
            InitializeComponent();
            DataContext = this;

            CboXe.ZNextFocus = TxtDungTich;
            CboLaiXe.ZNextFocus = TxtSdt;

            CmdSave = new RelayCommand(HandleSave, CanSave);

            DsXe = DbRepository.Instance.DsXe;
            DsLaiXe = DbRepository.Instance.DsLaiXe;

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ds.Filter.Run(null, true);
            SetTitles();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private async void HandleSave(object obj)
        {
            await Ds.Save(CurXe, CurLX);
            SetTitles();
        }
        private bool CanSave(object obj)
        {
            return (CurXe.IsChanged || CurLX.IsChanged || _xeid != CurXe.Id || _lxid != CurLX.Id) && !string.IsNullOrEmpty(CurXe.BSX) && !string.IsNullOrEmpty(CurLX.Ten);
        }

        private void LvXe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvXe.SelectedItem is KDXeLaiXeVM xlx)
            {
                if (xlx.Xe != null) CurXe.CopyFrom(xlx.Xe);
                else CurXe.Reset();
                CurXe.IsChanged = false;
                if (xlx.LaiXe != null) CurLX.CopyFrom(xlx.LaiXe);
                else CurLX.Reset();
                CurLX.IsChanged = false;

                _xeid = xlx.XeId;
                _lxid = xlx.LaiXeId;
            }
            else
            {
                _xeid = -1;
                _lxid = -1;
            }

            SetTitles();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO:
        }

        private void BtNew_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
            if (sender == BtTruckNew)
            {
                CurXe.Reset();
                LvXe.SelectedItem = null;
                CboXe.ZClearFilter();
            }
            else if (sender == BtDriverNew)
            {
                CurLX.Reset();
                CboLaiXe.ZClearFilter();
            }
            else if (sender == BtNew)
            {
                CurXe.Reset();
                CurLX.Reset();
                LvXe.SelectedItem = null;
                CboLaiXe.ZClearFilter();
                CboXe.ZClearFilter();
            }
            SetTitles();
        }

        private async void CheckSave()
        {
            if (CurXe.IsChanged || CurLX.IsChanged)
            {
                if (MessageBox.Show("Bạn có muốn lưu thông tin hiện tại?", "Lưu xe & lái xe", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await Ds.Save(CurXe, CurLX);
                }
            }
        }

        private void SetTitles()
        {
            if (CurXe.Id > 0) LblXeTitle.Text = $"Sửa thông tin: {CurXe.BSX}";
            else LblXeTitle.Text = "Tạo thông tin xe mới";
            if (CurLX.Id > 0) LblLaiXeTitle.Text = $"Sửa thông tin: {CurLX.Ten}";
            else LblLaiXeTitle.Text = "Tạo thông tin lái xe mới";
        }

        #region ComboBoxes
        private void CboXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided && e.Selected is KDXeVM xe)
            {
                CurXe.CopyFrom(xe);
                CurXe.IsChanged = false;
                SetTitles();
            }
        }

        private void CboLaiXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided && e.Selected is KDLaiXeVM xe)
            {
                CurLX.CopyFrom(xe);
                CurLX.IsChanged = false;
                SetTitles();
            }
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
