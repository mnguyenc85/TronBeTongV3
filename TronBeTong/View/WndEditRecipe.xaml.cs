using NMWPFControls.Core;
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
using System.Windows.Input;
using System.Windows.Media;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndEditMark.xaml
    /// </summary>
    public partial class WndEditRecipe : Window, INotifyPropertyChanged
    {
        #region Nguyen lieu
        private DelayUpdate<string> _filterSiloNL;
        public ObservableCollection<SiloNguyenLieuVM> DsNL { get; set; }
        private CollectionViewSource _cvsDsNL = new();
        public ICollectionView CVDsNL { get { return _cvsDsNL.View; } }
        public SiloNguyenLieuVM? SelectedNL { get; set; }
        #endregion

        public ObservableCollection<BTCongThucVM> DsCongThuc { get; set; }
        public BTCongThucVM CurCT { get; set; } = new();

        public HashSet<string> DsEditedRecipes { get; private set; } = [];

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndEditRecipe()
        {
            InitializeComponent();
            DataContext = this;

            DsNL = DbRepository.Instance.DsNguyenLieu;
            _cvsDsNL.Source = DsNL;
            _cvsDsNL.SortDescriptions.Add(new SortDescription(nameof(SiloNguyenLieuVM.PhanLoai), ListSortDirection.Ascending));
            _cvsDsNL.SortDescriptions.Add(new SortDescription(nameof(SiloNguyenLieuVM.Ten), ListSortDirection.Ascending));
            _filterSiloNL = new(300)
            {
                ExecUpdate = f => {
                    CVDsNL.Filter = x =>
                    {
                        if (x is SiloNguyenLieuVM c)
                        {
                            if (string.IsNullOrEmpty(f) || (c.Ten != null && c.Ten.Contains(f, StringComparison.CurrentCultureIgnoreCase))
                                || (c.Ma != null && c.Ma.Contains(f, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                    NguyenLieu_UpdateSTT();
                }
            };

            DsCongThuc = DbRepository.Instance.DsCongThuc;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _filterSiloNL.Run(null, true);

            // Tính KL các thành phần theo silo cho tất cả các công thức
            // Có thể dẫn đến thời gian load chậm nếu số lượng công thức lớn
            var chsilo = CauHinhTramTron.Instance;
            foreach (var ct in DsCongThuc)
            {
                if (ct.KLSilos == null)
                {
                    await DbRepository.Instance.CongThuc_LoadThanhPhan(ct);
                    chsilo.CheckCongThuc(ct);
                    ct.KLSilos = chsilo.TinhKLTheoSilo(ct);
                }
            }

            if (LvCongThuc.View is GridView gv)
            {
                foreach (var sl in chsilo.DsSilos)
                {
                    if (sl.NguyenLieu != null)
                    {
                        StackPanel pnlCol = new StackPanel() { MinWidth = 36 };
                        TextBlock lbl1 = new TextBlock() { Text = sl.Ma, Foreground = Brushes.LightGray };
                        pnlCol.Children.Add(lbl1);
                        TextBlock lbl2 = new TextBlock() { Text = sl.NguyenLieu.Ma, FontWeight = FontWeights.Bold };
                        pnlCol.Children.Add(lbl2);
                        switch (sl.NguyenLieu.PhanLoai)
                        {
                            case Core.LoaiThanhPhan.CotLieu:

                                gv.Columns.Add(new GridViewColumn
                                {
                                    //Header = string.Format("{0}\r\n{1}", sl.Ma, sl.NguyenLieu != null ? sl.NguyenLieu.Ma : null),
                                    Header = pnlCol,
                                    DisplayMemberBinding = new Binding($"KLSilos.CLs[{sl.Index}]")
                                });
                                break;
                            case Core.LoaiThanhPhan.XiMang:
                                gv.Columns.Add(new GridViewColumn
                                {
                                    //Header = string.Format("{0}\r\n{1}", sl.Ma, sl.NguyenLieu != null ? sl.NguyenLieu.Ma : null),
                                    Header = pnlCol,
                                    DisplayMemberBinding = new Binding($"KLSilos.Xis[{sl.Index}]")
                                });
                                break;
                            case Core.LoaiThanhPhan.PhuGia:
                                gv.Columns.Add(new GridViewColumn
                                {
                                    //Header = string.Format("{0}\r\n{1}", sl.Ma, sl.NguyenLieu != null ? sl.NguyenLieu.Ma : null),
                                    Header = pnlCol,
                                    DisplayMemberBinding = new Binding($"KLSilos.PGs[{sl.Index}]")
                                });
                                break;
                        }
                    }
                }
                gv.Columns.Add(new GridViewColumn
                {
                    Header = "Nước",
                    DisplayMemberBinding = new Binding($"KLSilos.Nuoc")
                });
                gv.Columns.Add(new GridViewColumn
                {
                    Header = new TextBlock() { Text = "Ngoài silo", Foreground = Brushes.Red },
                    DisplayMemberBinding = new Binding($"KLSilos.KLKhac")
                });
            }

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        public void LoadCongThuc(string? ma)
        {
            var ct = DsCongThuc.FirstOrDefault(x => x.Ma == ma);
            LvCongThuc.SelectedItem = ct;

            if (ct != null) _tongklct = ct.CalTotalWeight();
            else _tongklct = 0;
            UpdateKLBu();
        }

        #region Thành phần nguyên liệu
        private void TxtNLFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _filterSiloNL.Run(TxtNLFilter.Text);
        }

        private void BtAddNL_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNL != null)
            {
                CurCT.AddThanhPhan(SelectedNL);
            }
        }

        private void BtTP4Silos_Click(object sender, RoutedEventArgs e)
        {
            CauHinhTramTron DsCH = CauHinhTramTron.Instance;
            foreach (var s in DsCH.DsSilos)
            {
                if (s.NguyenLieu != null)
                {
                    var nl = CurCT.DsThanhPhan.FirstOrDefault(x => x.NL?.Ma == s.NguyenLieu.Ma);
                    if (nl == null)
                        CurCT.AddThanhPhan(s.NguyenLieu);
                }
            }
            UpdateTongKLCT();
        }

        private void BtTPXoa_Click(object sender, RoutedEventArgs e)
        {
            if (LvTP.SelectedItem is BTThanhPhanVM tp)
            {
                if (tp.State != Core.ViewModelStates.Remove)
                {
                    CurCT.RemoveThanhPhan(tp);
                }
            }
        }

        private void BtTPHuyXoa_Click(object sender, RoutedEventArgs e)
        {
            if (LvTP.SelectedItem is BTThanhPhanVM tp)
            {
                if (tp.State == Core.ViewModelStates.Remove)
                {
                    if (tp.Id > 0) tp.State = Core.ViewModelStates.None;
                    else tp.State = Core.ViewModelStates.Add;
                }
            }
        }

        private void BtTPKiemTra_Click(object sender, RoutedEventArgs e)
        {
            CauHinhTramTron.Instance.CheckCongThuc(CurCT);
        }

        private void NguyenLieu_UpdateSTT()
        {
            int stt = 1;
            foreach (var item in CVDsNL.Cast<SiloNguyenLieuVM>())
            {
                item.STT = stt++;
            }
        }

        private void TxtKLNuoc_ValueChanged(object sender, double e)
        {
            CurCT.CalWCRatio();
        }

        private void TxtTPKL_ValueChanged(object sender, double e)
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.DataContext is BTThanhPhanVM tp)
                {
                    tp.KL = e;
                    if (tp.NL?.PhanLoai == Core.LoaiThanhPhan.XiMang)
                    {
                        CurCT.CalWCRatio();
                    }
                    else if (tp.NL?.PhanLoai == Core.LoaiThanhPhan.Nuoc)
                    {
                        CurCT.CalWCRatio();
                    }

                    UpdateTongKLCT();
                }
            }
        }

        private double _tongkldat = 2400;
        private double _tongklct = 0;
        private void TxtTongKLDat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtTongKLDat == null || LblKLBu == null) return;
            if (double.TryParse(TxtTongKLDat.Text, out _tongkldat)) {
                UpdateKLBu();
            }
        }

        private void UpdateKLBu()
        {
            LblKLBu.Text = (_tongkldat - _tongklct).ToString();
        }

        private void UpdateTongKLCT()
        {
            _tongklct = CurCT.CalTotalWeight();
            LblTongKL.Text = _tongklct.ToString();
            double klbu = Math.Round(_tongkldat - _tongklct, 1);
            LblKLBu.Text = klbu.ToString();
        }
        #endregion

        #region Chuyển focus
        private void TxtTPKL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (sender is TextBox textBox)
                {
                    var container = FindAncestor<ListViewItem>(textBox);

                    if (container != null)
                    {
                        var listView = LvTP;
                        int index = listView.ItemContainerGenerator.IndexFromContainer(container);
                        if (index < listView.Items.Count - 1)
                        {
                            e.Handled = true; // Ngăn Tab mặc định

                            // Lấy ListViewItem tiếp theo
                            var nextItem = listView.ItemContainerGenerator.ContainerFromIndex(index + 1) as ListViewItem;
                            if (nextItem == null)
                            {
                                // Chưa sinh ra item -> cuộn vào để nó sinh ra
                                listView.ScrollIntoView(listView.Items[index + 1]);
                                listView.UpdateLayout();
                                nextItem = listView.ItemContainerGenerator.ContainerFromIndex(index + 1) as ListViewItem;
                            }

                            if (nextItem != null)
                            {
                                // Tìm TextBox trong hàng tiếp theo
                                var nextTextBox = FindVisualChild<TextBox>(nextItem);
                                if (nextTextBox != null)
                                {
                                    Keyboard.Focus(nextTextBox);
                                    nextTextBox.SelectAll();
                                }
                            }
                        }
                        // else: đang ở hàng cuối -> để mặc định Tab chuyển focus ra ngoài
                    }
                }
            }
        }

        public static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                else
                {
                    var result = FindVisualChild<T>(child);
                    if (result != null) return result;
                }
            }
            return null;
        }
        #endregion

        #region Công thức
        private bool _isSavingCT = false;
        private async void BtCTSave_Click(object sender, RoutedEventArgs e)
        {
            if (_isSavingCT) return;
            _isSavingCT = true;

            System.Diagnostics.Debug.WriteLine($"Start save ct -> Id = {CurCT.Id}");

            CurCT.STT = DsCongThuc.Count + 1;
            await DbRepository.Instance.CongThuc_Save(CurCT);

            if (!string.IsNullOrEmpty(CurCT.Ma) && !DsEditedRecipes.Contains(CurCT.Ma)) DsEditedRecipes.Add(CurCT.Ma);

            System.Diagnostics.Debug.WriteLine($"Saved ct -> Id = {CurCT.Id}");

            _isSavingCT = false;
        }

        private async void LvCongThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvCongThuc.SelectedItem is BTCongThucVM ct)
            {
                CurCT.CopyFrom(ct);
                await DbRepository.Instance.CongThuc_LoadThanhPhan(CurCT);                
                UpdateTongKLCT();
            }
        }

        private void BTCTNew_Click(object sender, RoutedEventArgs e)
        {
            BTCTNew.IsEnabled = false;
            CurCT.Reset();
            LvCongThuc.SelectedItem = null;
            UpdateTongKLCT();
            BTCTNew.IsEnabled = true;
        }

        private async void BTCTDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurCT.Id > 0)
            {
                await DbRepository.Instance.CongThuc_Delete(CurCT.Id);
                CurCT.Id = -1;
                UpdateTongKLCT();
            }
        }
        #endregion

        private void BtAddAppx_Click(object sender, RoutedEventArgs e)
        {
            CurCT.Slump += "±";
            TxtSlump.Focus();
            TxtSlump.CaretIndex = TxtSlump.Text.Length;
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
