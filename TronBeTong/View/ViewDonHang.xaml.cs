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
using NMComm.S71200;
using S7.Net;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel;
using TronBeTongV3.Data.ViewModel.DonHang;
using TronBeTongV3.Reports;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for ViewDonHang.xaml
    /// </summary>
    public partial class ViewDonHang : UserControl, INotifyPropertyChanged
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

        #region ZSelectedPhieu
        public object ZSelectedPhieu
        {
            get { return (object)GetValue(ZSelectedPhieuProperty); }
            set { SetValue(ZSelectedPhieuProperty, value); }
        }
        public static readonly DependencyProperty ZSelectedPhieuProperty =
            DependencyProperty.Register("ZSelectedPhieu", typeof(object), typeof(ViewDonHang), new PropertyMetadata(null));
        #endregion

        private KDDsXeVM _dsxe = new KDDsXeVM();

        /// <summary>
        /// Ngăn không truyền dữ liệu xuống PLC
        /// </summary>
        public bool LockPLC { get; set; }

        public ObservableCollection<KDXeVM> DsXe { get; set; }
        public ObservableCollection<KDLaiXeVM> DsLaiXe { get; set; }
        public ObservableCollection<BTCongThucVM> DsCongThuc { get; set; }

        public DHDonVM DonHang { get; private set; } = new DHDonVM();
        public DHPhieuVM Phieu { get; private set; } = new DHPhieuVM();

        private int _soloict = 0;

        public event EventHandler<ButtonArgs>? ButtonClick;

        public ViewDonHang()
        {
            InitializeComponent();
            DataContext = this;

            DsXe = _r.DsXe;
            DsLaiXe = _r.DsLaiXe;
            DsCongThuc = _r.DsCongThuc;

            CboXe.ZNextFocus = CboLaiXe.InnerTextBox;
            CboLaiXe.ZNextFocus = CboCongThuc.InnerTextBox;
            CboCongThuc.ZNextFocus = TxtM3;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EnableEditor();
        }

        public DHPhieuVM? ThayDoiDongHang(DHDonVM? d)
        {
            if (d == null)
            {
                DonHang.Reset();
                DonHang.DsPhieu.Clear();
                Phieu.DonHang = null;

                CboXe.ZClearSelected();
                CboLaiXe.ZClearSelected();

                EnableEditor();
                return null;
            }
            else
            {
                // Xóa xe, lái xe nếu đơn hàng thay đổi
                if (d.Id != DonHang.Id)
                {
                    CboXe.ZClearSelected();
                    CboLaiXe.ZClearSelected();
                    Phieu.LaiXe = null;
                    Phieu.Xe = null;
                }

                DonHang.CopyFrom(d);
                Phieu.DonHang = d;
                // NOTE: dự định load phiếu chưa hoàn thành tuy nhiên -> chạy lỗi -> tạm bỏ
                //await _r.DonHang_LoadAllPhieu(DonHang);
                //var ph = await _r.DonHang_LoadUnfinishedPhieu(DonHang);
                
                //if (ph != null)
                //{
                //    ph.DonHang = d;

                //    System.Diagnostics.Debug.WriteLine($"Chọn đơn hàng {d.Id}, có phiếu {ph.Id} chưa hoàn thành");
                //    // TODO: copy ph vào Phieu
                //    Phieu.CopyFrom(ph);
                //    Phieu.DonHang = d;
                //    CboXe.ZText = ph.Xe?.BSX;
                //    CboLaiXe.ZText = ph.LaiXe?.Ten;

                //    //CboCongThuc.ZText = ph.CongThuc?.Ma;
                //    //TxtSoMe.Text = ph.MeDat.ToString();
                //    //TxtM3.Text = ph.TheTichDat.ToString();
                //    var ct = DsCongThuc.FirstOrDefault(x => x.Ma == ph.CongThuc?.Ma);
                //    await ThayCongThuc(ct);

                //    TxtM3.Text = ph.TheTichDat.ToString();
                //    TxtSoMe.Text = ph.MeDat.ToString();
                //    Phieu.CongThuc?.ApplyM3(TxtM3.Value);
                //    if (Phieu.MeDat > 0)
                //    {
                //        Phieu.TheTichMe = Math.Round(Phieu.TheTichDat / Phieu.MeDat, 3);
                //    }
                //    if (!LockPLC)
                //    {
                //        ButtonClick?.Invoke(this, new ButtonArgs()
                //        {
                //            BtState = 1,
                //            Button = Core.ButtonTypes.GuiM3,
                //            ObjectId = 0
                //        });

                //        System.Diagnostics.Debug.WriteLine("Phieu: thay doi the tich tron");
                //    }
                //}
                //else
                //{
                //    System.Diagnostics.Debug.WriteLine($"Chọn đơn hàng {d.Id}, không có phiếu chưa hoàn thành");
                //    // TODO: xóa thông số phiếu đặt?

                //    Phieu.DonHang = d;
                //}

                EnableEditor();
                return null;
            }
        }

        private void CboXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                Phieu.Xe = e.Selected as KDXeVM;
                if (Phieu.Xe != null) {
                    CboXe.ZText = Phieu.Xe.BSX;
                    var xlx = _r.DsXLx.FirstOrDefault(x => x.XeId == Phieu.Xe.Id);
                    if (xlx != null)
                    {
                        CboLaiXe.ZSelectedItem = xlx.LaiXe;
                        CboLaiXe.ZText = xlx.LaiXe?.Ten;
                        Phieu.LaiXe = xlx.LaiXe;
                    }
                }
                EnableEditor();
            }
        }

        private void CboLaiXe_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is KDLaiXeVM lx)
                {
                    CboLaiXe.ZText = lx.Ten;
                    Phieu.LaiXe = lx;
                }
                EnableEditor();
            }
        }

        private async void CboCongThuc_ZSelectedChanged(object sender, NMWPFControls.Controls.NMCboSelectedChangedArgs e)
        {
            if (e.IsDecided)
            {
                if (e.Selected is BTCongThucVM ct)
                {
                    await ThayCongThuc(ct);
                }
                EnableEditor();
            }
        }
        private void CboCanPGRieng_Checked(object sender, RoutedEventArgs e)
        {
            if (!LockPLC)
            {
                ButtonClick?.Invoke(this, new ButtonArgs()
                {
                    BtState = 1,
                    Button = Core.ButtonTypes.GuiCapPhoi,
                    ObjectId = 0
                });

                System.Diagnostics.Debug.WriteLine($"Phieu: thay doi cong thuc {Phieu.CongThuc?.Id}, pg: {!Phieu.CanPGNgoai}");
            }
        }

        private void CboCanPGRieng_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!LockPLC)
            {
                ButtonClick?.Invoke(this, new ButtonArgs()
                {
                    BtState = 1,
                    Button = Core.ButtonTypes.GuiCapPhoi,
                    ObjectId = 0
                });

                System.Diagnostics.Debug.WriteLine($"Phieu: thay doi cong thuc {Phieu.CongThuc?.Id}, pg: {!Phieu.CanPGNgoai}");
            }
        }

        private async Task ThayCongThuc(BTCongThucVM? ct)
        {
            if (ct == null)
            {
                Phieu.CongThuc = null;
            }
            else
            {
                if (Phieu.CongThuc == null) Phieu.CongThuc = new DHCongThucVM();
                Phieu.CongThuc.CopyFrom(ct);
                await _r.CongThuc_LoadThanhPhan(Phieu.CongThuc);
                Phieu.CongThuc.Id = -1;                               // Xóa Id vì sẽ lưu ở bảng khác với BTCongThucDO
                Phieu.CongThuc.ApplyM3(TxtM3.Value);
                CheckCongThuc();

                CboCongThuc.ZText = Phieu.CongThuc.Ma;

                if (!LockPLC)
                {
                    ButtonClick?.Invoke(this, new ButtonArgs()
                    {
                        BtState = 1,
                        Button = Core.ButtonTypes.GuiCapPhoi,
                        ObjectId = 0
                    });

                    System.Diagnostics.Debug.WriteLine($"Phieu: thay doi cong thuc {Phieu.CongThuc?.Id}, pg: {!Phieu.CanPGNgoai}");
                }
            }
        }

        /// <summary>
        /// Gửi lại công thức hiện tại xuống PLC
        /// </summary>
        public async void GuiLaiCongThucHienTai()
        {
            if (CboCongThuc.ZSelectedItem is BTCongThucVM ct)
            {
                await ThayCongThuc(ct);
            }
            EnableEditor();
        }

        private void TxtM3_ValueChanged(object sender, double e)
        {
            ThayDoiTheTichTron();
        }

        private void TxtM3_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ThayDoiTheTichTron();
        }
        private void ThayDoiTheTichTron()
        {
            Phieu.TheTichDat = TxtM3.Value;
            Phieu.CongThuc?.ApplyM3(TxtM3.Value);
            if (Phieu.MeDat > 0)
            {
                Phieu.TheTichMe = Math.Round(Phieu.TheTichDat / Phieu.MeDat, 3);
            }

            if (!LockPLC)
            {
                ButtonClick?.Invoke(this, new ButtonArgs()
                {
                    BtState = 1,
                    Button = Core.ButtonTypes.GuiM3,
                    ObjectId = 0
                });

                System.Diagnostics.Debug.WriteLine("Phieu: thay doi the tich tron");
            }
        }

        private void TxtSoMe_ValueChanged(object sender, double e)
        {
            ThayDoiSoMeTron((int)e);
        }

        private void TxtSoMe_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ThayDoiSoMeTron((int)TxtSoMe.Value);
        }
        private void ThayDoiSoMeTron(int v)
        {
            Phieu.MeDat = v;
            Phieu.CongThuc?.ApplySoMe(v);
            if (v > 0)
            {
                Phieu.TheTichMe = Math.Round(Phieu.TheTichDat / v, 3);
            }

            if (!LockPLC)
            {
                ButtonClick?.Invoke(this, new ButtonArgs()
                {
                    BtState = 1,
                    Button = Core.ButtonTypes.GuiSoMe,
                    ObjectId = 0
                });

                System.Diagnostics.Debug.WriteLine("Phieu: thay doi me tron");
            }
        }

        //private async void BtSaveBill_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Phieu.DonHang == null)
        //    {
        //        MessageBox.Show("Phải chọn đơn hàng trước!", "Tạo phiếu", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }
        //    Phieu.SoPhieu = (DonHang.TongSoPhieu + 1).ToString();
        //    int id = await _r.Phieu_Save(Phieu);
        //    if (id > 0)
        //    {
        //        // Thêm phiếu vào đơn
        //        var ph1 = Phieu.Clone();
        //        ph1.Id = id;
        //        DonHang.AddPhieu(ph1);
        //        DonHang.TongSoPhieu = DonHang.DsPhieu.Count;
        //    }
        //}

        private void LblCongThucLoi1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Phieu.CongThuc != null)
            {
                WndEditRecipe wnd = new WndEditRecipe()
                {
                    Owner = this.Parent as Window,
                };
                wnd.LoadCongThuc(Phieu.CongThuc.Ma);
                wnd.ShowDialog();
                CheckCongThuc();
            }
        }

        /// <summary>
        /// Kiểm tra xem có thành phần nào của công thức không có silo tương ứng
        /// </summary>
        private void CheckCongThuc()
        {
            if (Phieu.CongThuc != null)
            {
                _soloict = CauHinhTramTron.Instance.CheckCongThuc(Phieu.CongThuc);
                LblCongThucLoi1.Visibility = _soloict > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public async Task SetCongThucTheoMa(string ma)
        {
            var ct = DsCongThuc.FirstOrDefault(c => c.Ma == ma);
            await ThayCongThuc(ct);
            EnableEditor();
        }

        private void TxtM3OverMe_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        public bool IsConnect { get; set; }
        /// <summary>
        /// Enable/disable controls
        /// </summary>
        public void EnableEditor()
        {
            if (!IsConnect || DonHang == null)
            {
                CboXe.IsEnabled = false;
                CboLaiXe.IsEnabled = false;
                CboCongThuc.IsEnabled = false;
                TxtM3.IsEnabled = false;
                TxtSoMe.IsEnabled = false;
            }
            else
            {
                CboXe.IsEnabled = true;
                CboLaiXe.IsEnabled = true;
                if (Phieu.Xe == null || Phieu.LaiXe == null)
                {
                    CboCongThuc.IsEnabled = false;
                    TxtM3.IsEnabled = false;
                    TxtSoMe.IsEnabled = false;
                }
                else
                {
                    CboCongThuc.IsEnabled = true;
                    //TxtM3.IsEnabled = Phieu.CongThuc != null;
                    //TxtSoMe.IsEnabled = Phieu.CongThuc != null;
                    TxtM3.IsEnabled = true;
                    TxtSoMe.IsEnabled = true;
                }
            }
        }

        private void BtOpenEditXe_Click(object sender, RoutedEventArgs e)
        {
            WndEditXe wnd = new();
            wnd.ShowDialog();
        }

        private async void BtSaveXe_Click(object sender, RoutedEventArgs e)
        {
            KDXeVM xe = new KDXeVM()
            {
                BSX = CboXe.ZText
                
            };
            KDLaiXeVM lx = new KDLaiXeVM()
            {
                Ten = CboLaiXe.ZText
            };
            await _dsxe.Save(xe, lx, true, true);

            EnableEditor();
        }
    }
}
