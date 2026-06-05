using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NMWPFControls.Core;
using NMComm;
using TronBeTongV3.CSDL;
using TronBeTongV3.View;
using TronBeTongV3.Core;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.ViewModel.DonHang;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Reports;
using TronBeTongV3.CSDL.Server;
using TronBeTongV3.Comm;
using Serilog;
using TronBeTongV3.Debugger;
using System.Threading.Tasks;

namespace TronBeTongV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbBridge _db = DbBridge.Instance;
        private DbSettings _s = DbRepository.Instance.Settings;

        private MainVM _vm = new();
        //private ModelTramTronBeTong _tramtron = new ModelTramTronHoangHai();
        private ModelHeThong _tramtron = new ModelTramTron3();

        private readonly RealtimeAverage _readcycle_avr = new(10);
        private int _update_delay = 0;
        private long _tmr_t0;
        private double _m3me;

        private bool _daAnNutDung = false;
        /// <summary>
        /// đã ấn nút reset chưa? chỉ tạo phiếu mới khi đã ấn nút này!
        /// </summary>
        private bool _daAnReset = true;

        private TagsObserver _debugObserver;

        #region Start & Shutdown
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.CtrlMsg = DebugMsg1;

            GlobalUIEvent.Instance.UIEventRaised += Instance_UIEventRaised;
            ViewDonHang.ButtonClick += ViewDonHang_ButtonClick;
            _vm.ActiveDon = ViewDonHang.DonHang;

            _vm.TramTron = _tramtron;
            TPCotLieu.TramTron = _tramtron;
            TPXiMang.TramTron = _tramtron;
            TPNuoc.TramTron = _tramtron;
            TPPhuGia.TramTron = _tramtron;
            CtrlMixer1.TramTron = _tramtron;
            TPBangTai.TramTron = _tramtron;
            CtrlThungCLTG.TramTron = _tramtron;
            
            _debugObserver = new TagsObserver(_tramtron);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PnlDebug.Visibility = Visibility.Collapsed;
            await Init();
            _tramtron.StartConnection();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopConnection();

            XuLyInPhieu.Instance.Close();
            //XuLyInBaoCao.Instance.Close();
            //_client.Stop();

            // Save settings before exit program
            var r = DbRepository.Instance;
            var s = r.Settings;
            s.UpdateBool("in.phieu.auto", ChkAutoPrint.IsChecked == true);
            s.UpdateDouble("in.phieu.auto.times", TxtPrintRepeat.Value);
            await _db.SaveSettingsAsync(s);
        }

        private async Task Init()
        {
            var db = DbBridge.Instance;
            var r = DbRepository.Instance;

            try
            {
                db.DbSrcName = Properties.Settings.Default.DbSrcName;
                db.Init();

                await db.LoadSettingsAsync(r.Settings);

                LblAddr.Text = _tramtron.SetComm();

                await r.LoadDataAsync();
                await CauHinhTramTron.Instance.Load();

                await db.PM_Activites_DelAsync();

                TPCotLieu.LoadThamSo(r.Settings);
                TPXiMang.LoadThamSo(r.Settings);
                TPNuoc.LoadThamSo(r.Settings);
                TPPhuGia.LoadThamSo(r.Settings);
            }
            catch
            {
                MessageBox.Show("Lỗi CSDL.\r\nVào 'Hệ thống' -> 'Cài đặt' để tạo CSDL", "Khởi tạo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Log.Information("Successful init database");
            
            ChkAutoPrint.IsChecked = r.Settings.GetBoolValue("in.phieu.auto");
            TxtPrintRepeat.Value = (int)r.Settings.GetDoubleValue("in.phieu.auto.times");

            bool showDebug = r.Settings.GetBoolValue("debug.visible");
            if (!showDebug) DebugMsg1.Visibility = Visibility.Collapsed;

            double appZoom = Math.Round(r.Settings.GetDoubleValue("app.zoom", 1), 2);
            _vm.AppZoom = appZoom;
            CheckMniViewZoom(appZoom);

            SetSilosNguyenLieu();

            _tramtron.CreateKEPServerNodes();
            Log.Information("Successful init KEP");
            BdrPLC.Visibility = _tramtron.ConnType == ConnectionTypes.KEPServer? Visibility.Visible: Visibility.Collapsed;

            InitServerSync();
            Log.Information("Successful init server");
        }

        private void SetSilosNguyenLieu()
        {
            for (int i = 0; i < CauHinhTramTron.MAX_CL; i++)
            {
                TPCotLieu.SetupBin(i);
            }
            for (int i = 0; i < CauHinhTramTron.MAX_XM; i++)
            {
                TPXiMang.SetupBin(i);
            }
            for (int i = 0; i < CauHinhTramTron.MAX_PG; i++)
            {
                TPPhuGia.SetupBin(i);
            }
        }

        #endregion

        #region Main loop
        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            Loop();
        }

        private bool _isUpdateView = false;
        private async void Loop()
        {
            if (_isUpdateView) return;

            _isUpdateView = true;
            long t = DateTime.Now.Ticks;
            double delta = (t - _tmr_t0) / 10000000d;
            _tmr_t0 = t;

            await UpdateCommState();

            if (_tramtron.IsRunning)
            {
                _tramtron.UpdateTags();

                await UpdateView(delta);

                TPPhuGia.Update(delta);
                TPXiMang.UpdateView(delta);
                TPCotLieu.Update(delta);
                TPNuoc.Update(delta);
                #region Chốt dữ liệu cân
                // Chốt dữ liệu ở đây dẫn đến chặn vòng lặp update giao diện
                // Có thể đổi sang 1 Task riêng
                // Như vậy cần có biên riêng để kiểm tra sự thay đổi trạng thái cân
                if (ViewDonHang.Phieu.DonHang != null && ViewDonHang.Phieu.DonHang.Id > 0)
                {
                    if (TPXiMang.CheckChotMe())
                    {
                        if (TPXiMang.WSCements1.NeedSaveWeights)
                        {
                            await ChotMe(TPXiMang.WSCements1.MeHT, 1, 0);
                            DebugMsg1.AddMessage($"Chốt xi măng 1 - mẻ {TPXiMang.WSCements1.MeHT}");
                        }
                        if (TPXiMang.WSCements2.NeedSaveWeights)
                        {
                            await ChotMe(TPXiMang.WSCements2.MeHT, 1, 0);
                            DebugMsg1.AddMessage($"Chốt xi măng 2 - mẻ {TPXiMang.WSCements2.MeHT}");
                        }
                    }
                    if (TPCotLieu.CheckChotMe())
                    {
                        if (TPCotLieu.WsTotalCL1.NeedSaveWeights)
                        {
                            await ChotMe(TPCotLieu.WsTotalCL1.MeHT, 1, 1);
                            DebugMsg1.AddMessage($"Chốt cốt liệu 1 - mẻ {TPCotLieu.WsTotalCL1.MeHT}");
                        }
                        if (TPCotLieu.WsTotalCL2.NeedSaveWeights)
                        {
                            await ChotMe(TPCotLieu.WsTotalCL2.MeHT, 1, 1);
                            DebugMsg1.AddMessage($"Chốt cốt liệu 2 - mẻ {TPCotLieu.WsTotalCL2.MeHT}");
                        }
                        if (TPCotLieu.WsTotalCL3.NeedSaveWeights)
                        {
                            await ChotMe(TPCotLieu.WsTotalCL3.MeHT, 1, 1);
                            DebugMsg1.AddMessage($"Chốt cốt liệu 3 - mẻ {TPCotLieu.WsTotalCL3.MeHT}");
                        }
                    }
                    if (TPNuoc.CheckChotMe())
                    {
                        await ChotMe(TPNuoc.WSWater.MeHT, 1, 2);
                        DebugMsg1.AddMessage($"Chốt nước - mẻ {TPNuoc.WSWater.MeHT}");
                    }
                    if (TPPhuGia.CheckChotMe())
                    {
                        await ChotMe(TPPhuGia.WScale1.MeHT, 1, 3);
                        DebugMsg1.AddMessage($"Chốt pg - mẻ {TPPhuGia.WScale1.MeHT}");
                    }
                }
                #endregion
                TPBangTai.UpdateView(delta);
                CtrlThungCLTG.UpdateView(delta);
                CtrlMixer1.Update(delta);

                _tramtron.MarkUpdateView();

                _debugObserver.CollectData();
            }

            _readcycle_avr.AddVal(delta);
            if (--_update_delay <= 0)
            {
                double t1 = (DateTime.Now.Ticks - t) / 10000d;
                LblViewCycle.Text = string.Format("View: {0:0.000}/{1:0.000} ms", t1, _readcycle_avr.Mean * 1000);
                LblCycle.Text = string.Format("PLC: {0:0.000} ms", _tramtron.RealCycle * 1000d);

                _update_delay = 10;
            }
            _isUpdateView = false;
        }

        private async Task UpdateCommState()
        {
            if (_tramtron.CheckCommState())
            {
                switch (_tramtron.CommState)
                {
                    case CommStates.Closed:
                        LblConn.Text = "Không kết nối";
                        LblConn.Foreground = Brushes.White;
                        BdrConn.Background = Brushes.Red;
                        break;
                    case CommStates.Openning:
                        LblConn.Text = "Đang kết nối";
                        LblConn.Foreground = Brushes.Black;
                        BdrConn.Background = Brushes.Yellow;
                        break;
                    case CommStates.Opened:
                        LblConn.Text = "Kết nối";
                        LblConn.Foreground = Brushes.Black;
                        BdrConn.Background = Brushes.LightGreen;
                        break;
                    case CommStates.None:
                        LblConn.Text = "Dừng kết nối";
                        BdrConn.Background = Brushes.Wheat;
                        LblConn.Foreground = Brushes.Black;
                        break;
                }
                await _db.PM_Activites_SaveAsync(1, 3, _tramtron.CommState.ToString());
            }
            
            if (_tramtron.ConnInfo != null && _tramtron.ConnInfo.IsChanged)
            {
                switch (_tramtron.ConnInfo.State)
                {
                    case DeviceConnStates.None:
                        BdrPLC.Background = Brushes.Transparent;
                        break;
                    case DeviceConnStates.Good:
                        BdrPLC.Background = Brushes.LightGreen;
                        break;
                    case DeviceConnStates.Disconnect:
                        BdrPLC.Background = Brushes.Red;                        
                        break;
                }
                LblPLC.Text = _tramtron.ConnInfo.State.ToString();
                await _db.PM_Activites_SaveAsync(1, 3, _tramtron.ConnInfo.State.ToString());
            }

            ViewDonHang.IsConnect = _tramtron.CommState == CommStates.Opened;
            ViewDonHang.EnableEditor();
        }

        private bool _nap_nl0 = false;
        private long _dung_nap_t0;
        private bool _running0 = false;
        private bool _is_sim0 = false;
        private bool _ketthucdinhluong0 = false;
        private DateTime _chay_t0 = DateTime.MinValue;
        private readonly TimeSpan _tgTreChot = new TimeSpan(0, 0, 3);
        private async Task UpdateView(double delta)
        {
            LEDWeightMode.IsOn = !_tramtron.CheDoCan.GetBool();
            LEDMixCompleleted.IsOn = _tramtron.MixerCompleted.GetBool();
            LEDRunning.IsOn = _tramtron.SysRunning.GetBool();
            LEDSkipMode.IsOn = _tramtron.CheDoSkip.GetBool();

            #region Nạp nguyên liệu & chốt (ko dùng)
            bool nap_nl = _tramtron.NapNguyenLieu.GetBool();
            LEDMixStarted.IsOn = nap_nl;
            if (!_nap_nl0 && nap_nl)
            {
                //DebugMsg1.AddMessage("Chốt mẻ");
                //ChotMe(1);
            }
            else if (_nap_nl0 && !nap_nl)
            {
                _dung_nap_t0 = DateTime.Now.Ticks;
            }
            //if (!nap_nl) {
            //    LblDebug.Text = $"Dừng nạp: {(DateTime.Now.Ticks - _dung_nap_t0) / 10000000d} s";
            //}
            _nap_nl0 = nap_nl;
            #endregion

            #region Trạng thái cân/dừng
            bool running = _tramtron.SysRunning.GetBool();
            ViewDonHang.LockPLC = running;
            BtRun.Visibility = running ? Visibility.Hidden : Visibility.Visible;
            BtStop.Visibility = running ? Visibility.Visible : Visibility.Hidden;
            if (_running0 && !running)
            {
                // Chỉ chốt mẻ khi đã chạy it nhất 3 s
                if (_chay_t0 > DateTime.MinValue && (DateTime.Now - _chay_t0) > _tgTreChot)
                {
                    DebugMsg1.AddMessage("Chốt mẻ khi dừng chạy");
                    int sttme = Math.Max(Math.Max(TPXiMang.MeMax, TPCotLieu.MeMax), TPNuoc.MeMax);
                    // chỉ chốt khi cối trộn chạy?
                    //if (_tramtron.MixerRunning.GetBool())
                    await ChotMe(sttme, 2, 3);
                    _vm.InPhieu(ChkAutoPrint.IsChecked == true, TxtPrintRepeat.Value, TxtPhieuSeal.Text);
                }

                TPNuoc.EnableWasher(true);

                _vm.SetTTDonHang(1);

                await _db.PM_Activites_SaveAsync(2, 2, "Dừng cân");

                // Khi hệ thống dừng tự động (không ấn nút dừng) -> coi như đã ấn nút reset -> tạo phiếu mới
                if (!_daAnNutDung)
                {
                    _daAnReset = true;
                    ViewDonHang.IsEnabled = true;
                }

                // Tắt mô phỏng
                _tramtron.WriteSysSim(0);
            }
            else if (!_running0 && running)
            {
                DebugMsg1.AddMessage("Kiểm tra tạo phiếu mới khi bắt đầu chạy?");
                TaoPhieu(_daAnReset);
                _chay_t0 = DateTime.Now;
                _daAnNutDung = false;

                TPNuoc.EnableWasher(false);

                _vm.SetTTDonHang(2);
                ViewDonHang.IsEnabled = false;

                await _db.PM_Activites_SaveAsync(2, 1, "Cân");
            }
            _running0 = running;
            #endregion

            bool _is_sim = _tramtron.IsSim;
            if (_is_sim != _is_sim0)
            {
                LEDSimulation.IsOn = _is_sim;
                BtSimulation.Content = _is_sim ? "Tắt mô phỏng" : "Chạy mô phỏng";
                _is_sim0 = _is_sim;
            }

            LEDCementDischargeMode.IsOn = !_tramtron.CheDoXa.GetBool();
            LEDConcreteDischargeMode.IsOn = !_tramtron.CheDoXaBeTong.GetBool();

            UpdateOnlineMonitor();

            BaoPCChoPhepCan();
        }
        #endregion

        #region Menu
        private void MniDataAggre_Click(object sender, RoutedEventArgs e)
        {
            LoaiThanhPhan t = LoaiThanhPhan.None;
            if (sender == MniDataAggregates) t = LoaiThanhPhan.CotLieu;
            else if (sender == MniDataCements) t = LoaiThanhPhan.XiMang;
            else if (sender == MniDataAdditives) t = LoaiThanhPhan.PhuGia;
            ShowEditSiloWnd(t, -1);
        }

        private void Instance_UIEventRaised(object? sender, GlobalUIEventArgs e)
        {
            if (e.Kind == GlobalUIEventKinds.CauHinhCotLieu)
            {
                ShowEditSiloWnd(LoaiThanhPhan.CotLieu, e.ObjectId);
            }
            else if (e.Kind == GlobalUIEventKinds.CauHinhPhuGia)
            {
                ShowEditSiloWnd(LoaiThanhPhan.PhuGia, e.ObjectId);
            }
            else if (e.Kind == GlobalUIEventKinds.CauHinhXiMang)
            {
                ShowEditSiloWnd(LoaiThanhPhan.XiMang, e.ObjectId);
            }
            else if (e.Kind == GlobalUIEventKinds.DebugMsg && !string.IsNullOrEmpty(e.Text))
            {
                DebugMsg1.AddMessage(e.Text);
            }
        }

        private void ShowEditSiloWnd(LoaiThanhPhan loai, int oid)
        {
            WndEditSilo wnd = new()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                TPCotLieu = TPCotLieu,
                TPXiMang = TPXiMang,
            };
            wnd.SelectSilo(loai, oid);
            wnd.ShowDialog();

            SetSilosNguyenLieu();
        }

        private void MniDataCustomers_Click(object sender, RoutedEventArgs e)
        {
            WndEditKhachHang wnd = new()
            {
                Owner = this
            };
            wnd.ShowDialog();
            StartSync();
        }

        private void MniDataProjects_Click(object sender, RoutedEventArgs e)
        {
            WndEditDuAn wnd = new()
            {
                Owner = this
            };
            wnd.ShowDialog();
        }

        private void MniDataTrucks_Click(object sender, RoutedEventArgs e)
        {
            WndEditXe wnd = new()
            {
                Owner = this
            };
            wnd.ShowDialog();
            StartSync();
        }

        private void MniSysConfig_Click(object sender, RoutedEventArgs e)
        {
            var r = DbRepository.Instance;

            int quyenhan = 0;
            WndLogin login = new()
            {
                Owner = this,
            };
            if (login.ShowDialog() == true)
            {
                // Check password
                if (login.Account != null && login.Account.UserName != null && login.Account.Password != null)
                {
                    var ktacc = r.Settings.GetValue("pm.quantri.kythuat.account");
                    var ktpw = r.Settings.GetValue("pm.quantri.kythuat.password");
                    string pw = MyCopyright.ComputeSha256(login.Account.Password);
                    if (string.IsNullOrWhiteSpace(ktacc))
                    {
                        r.Settings.Update($"pm.quantri.kythuat.account", login.Account.UserName);
                        r.Settings.Update($"pm.quantri.kythuat.password", pw);
                        quyenhan = 255;
                    }
                    else
                    {
                        if (ktpw == pw) quyenhan = 255;
                    }
                }

                WndConfig wnd = new()
                {
                    Owner = this,
                };
                wnd.SetPrivilege(quyenhan);
                if (wnd.ShowDialog() == true)
                {
                    LblAddr.Text = _tramtron.SetComm();
                    InitServerSync();
                }
            }
        }

        private void MniDataRecipes_Click(object sender, RoutedEventArgs e)
        {
            WndEditRecipe wnd = new()
            {
                Owner = this,
            };
            if (wnd.ShowDialog() == true)
            {
            }
            // Gửi lại cấp phối nếu như cấp phối hiện tại bị sửa (có trong ds những công thức bị sửa)
            if (!string.IsNullOrEmpty(ViewDonHang.Phieu.CongThuc?.Ma) && wnd.DsEditedRecipes.Contains(ViewDonHang.Phieu.CongThuc.Ma))
            {
                var ct = ViewDonHang.Phieu.CongThuc;
                if (ct != null)
                {
                    ViewDonHang.GuiLaiCongThucHienTai();
                }
            }
        }

        private WndThongKe? _tkwnd;
        private void MnuThongKe_Click(object sender, RoutedEventArgs e)
        {
            if (_tkwnd == null)
            {
                _tkwnd = new WndThongKe();
                _tkwnd.Closed += (o, _) =>
                {
                    _tkwnd = null;
                };
            }
            _tkwnd.Show();
        }

        private void MniSysExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MniRegister_Click(object sender, RoutedEventArgs e)
        {
            WndRegister wnd = new()
            {
                Owner = this,
            };
            wnd.ShowDialog();
        }
        private void MniSetParams_Click(object sender, RoutedEventArgs e)
        {
            WndDatThamSo wnd = new() {
                Owner = this, 
                TramTron = _tramtron
            };
            wnd.ShowDialog();
        }
        private void MniAbout_Click(object sender, RoutedEventArgs e)
        {
            WndAbout wnd = new() { Owner = this };
            wnd.ShowDialog();
        }

        private void MniViewZoom75_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(0.75);
        }

        private void MniViewZoom100_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(1);
        }
        private void MniViewZoom110_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(1.1);
        }
        private void MniViewZoom120_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(1.2);
        }
        private void MniViewZoom125_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(1.25);
        }

        private void MniViewZoom150_Click(object sender, RoutedEventArgs e)
        {
            SetAppZoom(1.5);
        }

        private bool _skipSetZoom = false;
        private void CheckMniViewZoom(double dz)
        {
            if (_skipSetZoom) return;
            _skipSetZoom = true;
            double z = Math.Round(dz, 2);
            ResetViewChecked();
            switch (z)
            {
                case 0.75:
                    MniViewZoom75.IsChecked = true;
                    break;
                case 1.1:
                    MniViewZoom110.IsChecked = true;
                    break;
                case 1.2:
                    MniViewZoom120.IsChecked = true;
                    break;
                case 1.25:
                    MniViewZoom125.IsChecked = true;
                    break;
                case 1.5:
                    //MniViewZoom150.IsChecked = true;
                    break;
                default:
                    MniViewZoom100.IsChecked = true;
                    break;
            }
            _skipSetZoom = false;
        }

        private async void SetAppZoom(double z)
        {
            if (z > 0.5)
            {
                _vm.AppZoom = z;
                CheckMniViewZoom(z);
                var s = DbRepository.Instance.Settings;
                s.UpdateDouble("app.zoom", z);
                await _db.SaveSettingsAsync(s);
            }
        }

        private void ResetViewChecked()
        {
            MniViewZoom75.IsChecked = false;
            MniViewZoom100.IsChecked = false;
            MniViewZoom110.IsChecked = false;
            MniViewZoom120.IsChecked = false;
            MniViewZoom125.IsChecked = false;
        }
        #endregion

        #region UI
        private async void BdrConn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_tramtron.CanCommunicate)
            {
                StopConnection();
                await _db.PM_Activites_SaveAsync(1, 2, "Stop Comm");
            }
            else
            {
                _tramtron.StartConnection(true);
                await _db.PM_Activites_SaveAsync(1, 1, "Start Comm");
            }
        }

        public void StopConnection()
        {
            _tramtron.StopConnection();

            LblConn.Text = "Dừng kết nối";
            BdrConn.Background = Brushes.Wheat;
            LblConn.Foreground = Brushes.Black;
        }
        #endregion

        #region Business
        private void BtOrderCreate_Click(object sender, RoutedEventArgs e)
        {
            WndCreateOrder wnd = new()
            {
                Owner = this,
            };
            wnd.ShowDialog();
            StartSync();
        }

        private void TxtDonHangFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.DonOnlineFilter.Run(TxtDonHangFilter.Text);
        }

        private void BtMarkActiveDon_Click(object sender, RoutedEventArgs e)
        {
            _vm.MarkActiveOrder();
            _vm.DonOnlineFilter.Run(TxtDonHangFilter.Text);
        }

        private void BtViewDonHang_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.DataContext is DHDonVM don)
                {
                    WndViewDonHang wnd = new WndViewDonHang();
                    wnd.DonHang.CopyFrom(don);
                    wnd.Show();
                }
            }
        }

        private DHDonVM? _lastDon = null;
        private async void LvDonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LEDRunning.IsOn)
            {
                DebugMsg1.AddMessage("Đang chạy không thay đổi được đơn hàng!");
                e.Handled = true;
                return;
            }
            else
            {
                if (!_daAnReset)
                {
                    MessageBox.Show("Đang tạm dừng không thay đổi được đơn hàng!", "Thay đổi đơn!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    e.Handled = true;
                    return;
                }
            }

            if (_tramtron.CommState != CommStates.Opened && LvDonHang.SelectedItem != null)
            {
                DebugMsg1.AddMessage("Không thay đổi được đơn hàng\r\nLỗi: Không kết nối PLC!");
                //MessageBox.Show("Lỗi: Không kết nối PLC", "Chọn đơn hàng", MessageBoxButton.OK, MessageBoxImage.Error);
                LvDonHang.SelectedItem = null;
                return;
            }

            var don = LvDonHang.SelectedItem as DHDonVM;

            if (don != _lastDon)
            {
                if (_lastDon != null) _lastDon.TrangThai = 0;
                if (don != null) { 
                    don.TrangThai = 1;
                }
                ViewDonHang.ThayDoiDongHang(don);
                _vm.ClearActivePhieu();
                
                if (_s.GetBoolValue("auto.congthuc.donhang") && don != null)
                {
                    string? ct_ma = await _db.HT_CongThuc_ByLastDonHang(don.Id);
                    if (ct_ma != null)
                    {
                        await ViewDonHang.SetCongThucTheoMa(ct_ma);
                    }
                }

                _lastDon = don;
            }
            

            // Tìm phiếu chưa hoàn thành và đặt làm phiếu hiện tại?
            //var ph = await ViewDonHang.ThayDoiDongHang(don);
            //if (ph != null) _vm.SetActivePhieu(ph, false);
            //else _vm.ClearActivePhieu();

            System.Diagnostics.Debug.WriteLine($"Phiếu active: {_vm.ActivePhieu?.Id}, thuộc đơn hàng: {_vm.ActivePhieu?.DonHang?.Id}");
        }

        private void LvDonHang_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ViewDonHang_ButtonClick(object? sender, ButtonArgs e)
        {
            if (e.Button == ButtonTypes.GuiCapPhoi)
            {
                var ph = ViewDonHang.Phieu;
                var ct = ViewDonHang.Phieu.CongThuc;
                bool canPGNgoai = ViewDonHang.Phieu.CanPGNgoai;
                if (canPGNgoai) TPPhuGia.SetCanPGNgoai(ct, ph.TheTichDat, ph.MeDat);
                else TPPhuGia.SetCanPG();
                if (ct != null)
                {
                    _tramtron.WriteCapPhoi(ct, !canPGNgoai);
                }
            }
            else if (e.Button == ButtonTypes.GuiM3)
            {
                var ph = ViewDonHang.Phieu;
                var ct = ph.CongThuc;
                bool canPGNgoai = ViewDonHang.Phieu.CanPGNgoai;
                if (canPGNgoai) TPPhuGia.SetCanPGNgoai(ct, ph.TheTichDat, ph.MeDat);
                else TPPhuGia.SetCanPG();
                if (ct != null)
                {
                    System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} {ph.TheTichDat} / {ph.MeDat} = {ph.TheTichMe}");
                    //_tramtron.S71200_WriteM3(ph.TheTichDat);
                    _tramtron.WriteTag(_tramtron.MeM3Dat, ph.TheTichDat);
                }
            }
            else if (e.Button == ButtonTypes.GuiSoMe)
            {
                var ph = ViewDonHang.Phieu;
                var ct = ph.CongThuc;
                bool canPGNgoai = ViewDonHang.Phieu.CanPGNgoai;
                if (canPGNgoai) TPPhuGia.SetCanPGNgoai(ct, ph.TheTichDat, ph.MeDat);
                else TPPhuGia.SetCanPG();
                if (ct != null)
                {
                    System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} {ph.TheTichDat} / {ph.MeDat} = {ph.TheTichMe}");
                    //_tramtron.S71200_WriteSoMe(ph.MeDat);
                    _tramtron.WriteTag(_tramtron.MeSoMe, ph.MeDat);
                }
            }
        }

        private void BtSetMixerParams_Click(object sender, RoutedEventArgs e)
        {
            if (TxtTGTron.Value <= 0 || TxtTGXa.Value <= 0)
            {
                MessageBox.Show("Lỗi đặt tham số thời gian (phải > 0)", "Đặt tham số thời gian", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _tramtron.WriteMixerTG((int)TxtTGTron.Value, (int)TxtTGXa.Value, (int)TxtTGXaNua.Value);
        }

        private DateTime _lastTimeSend = DateTime.Now;
        /// <summary>
        /// Nếu có đơn hàng và công thức -> gữi 1 xung 300ms, chu kỳ 1000ms
        /// </summary>
        private void BaoPCChoPhepCan()
        {
            if (_tramtron.CommState == CommStates.Opened && ViewDonHang.Phieu.CongThuc != null && ViewDonHang.Phieu.DonHang != null && ViewDonHang.Phieu.LaiXe != null && ViewDonHang.Phieu.Xe != null)
            {
                if (DateTime.Now - _lastTimeSend > TimeSpan.FromSeconds(3))
                {
                    _tramtron.S71200_WritePCChoPhepCan();
                    _lastTimeSend = DateTime.Now;
                }
            }
        }
        private void BtInPhieu_Click(object sender, RoutedEventArgs e)
        {
            _vm.InPhieu(true, TxtPrintRepeat.Value, TxtPhieuSeal.Text);
        }
        #endregion

        #region PLC commands
        private async void BtRun_Click(object sender, RoutedEventArgs e)
        {
            if (CtrlMixer1.DischargeSetTime < 1 || CtrlMixer1.MixSetTime < 1)
            {
                //DebugMsg1.AddMessage("Lỗi: chưa đặt thời gian trộn!");
                MessageBox.Show("Lỗi: Chưa đặt thời gian trộn!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ViewDonHang.DonHang == null || ViewDonHang.DonHang.Id <= 0)
            {
                MessageBox.Show("Lỗi: Chưa chọn đơn hàng!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ViewDonHang.Phieu.Xe == null)
            {
                MessageBox.Show("Lỗi: Chưa chọn xe!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_tramtron.SysWashMixer.GetBool())
            {
                MessageBox.Show("Lỗi: Đang rửa cối trộn!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TPCotLieu.CheckCanDay() > 0)
            {
                MessageBox.Show("Lỗi: Có cân cốt liệu ở trạng thái đầy!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TPXiMang.CheckCanDay() > 0)
            {
                MessageBox.Show("Lỗi: Có cân xi măng ở trạng thái đầy!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TPNuoc.CheckCanDay() > 0)
            {
                MessageBox.Show("Lỗi: Cân nước ở trạng thái đầy!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TPPhuGia.CheckCanDay() > 0)
            {
                MessageBox.Show("Lỗi: Cân phụ gia ở trạng thái đầy!", "Bắt đầu chạy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DebugMsg1.AddMessage("Bắt đầu chạy");
            //_tramtron.S71200_WriteStart();
            _tramtron.WriteTag(_tramtron.SysRunning, 1);
            TxtPhieuSeal.Text = "";

            await _db.PM_Activites_SaveAsync(3, 1, "Ấn chạy");
        }

        private async void BtStop_Click(object sender, RoutedEventArgs e)
        {
            //_tramtron.S71200_WriteStart(0);
            _tramtron.WriteTag(_tramtron.SysRunning, 0);
            _daAnNutDung = true;
            //BtRun.Content = "Trộn tiếp";

            await _db.PM_Activites_SaveAsync(3, 2, "Ấn dừng");
        }

        private void BtReset_Click(object sender, RoutedEventArgs e)
        {
            //_tramtron.S71200_WriteReset();
            //_tramtron.WriteSysReset();
        }

        private void BtReset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_tramtron.SysRunning.GetBool()) return;

            _tramtron.WriteTag(_tramtron.SysReset, 1);
            _daAnReset = true;
            //BtRun.Content = "Bắt đầu trộn";
            if (!_tramtron.SysRunning.GetBool())
            {
                ViewDonHang.IsEnabled = true;
            }
        }

        private void BtReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_tramtron.SysRunning.GetBool()) return;

            _tramtron.WriteTag(_tramtron.SysReset, 0, 0.1);
        }

        /// <summary>
        /// Bật tắt chạy mô phỏng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (!LEDSimulation.IsOn)
            {
                WndEnterPass wnd = new WndEnterPass() { Owner = this, PassType = "sim.pw" };
                if (wnd.ShowDialog() != true)
                {
                    return;
                }
                //_tramtron.S71200_WriteSimulation(1);
                _tramtron.WriteSysSim(1);
            }
            else
            {
                //_tramtron.S71200_WriteSimulation(0);
                _tramtron.WriteSysSim(0);
            }
        }

        private void BtKetThucDinhLuong_Click(object sender, RoutedEventArgs e)
        {
            //_tramtron.WriteTag(_tramtron.BamKetThucDinhLuong, 1);
        }
        #endregion

        #region Lưu dữ liệu
        /// <summary>
        /// Kiểm tra điều kiện và tạo phiếu
        /// </summary>
        /// <param name="forceNew">Bắt buộc tạo phiếu mới</param>
        /// <returns></returns>
        private int TaoPhieu(bool forceNew)
        {
            _daAnReset = false;

            //ViewDonHang.Phieu.TheTichDat = _plc.Db6SoMe.M3Dat.Value;
            //ViewDonHang.Phieu.MeDat = (int)_plc.Db6SoMe.SoMeDat.Value;
            ViewDonHang.Phieu.TheTichDat = _tramtron.MeM3Dat.Value;
            ViewDonHang.Phieu.MeDat = (int)_tramtron.MeSoMe.Value;            
            if (ViewDonHang.Phieu.TheTichDat <= 0 || ViewDonHang.Phieu.MeDat <= 0)
            {
                DebugMsg1.AddMessage("Lỗi tạo phiếu: Mẻ đặt & M3 đặt phải > 0");
                return -3;
            }
            int err = _vm.SetActivePhieu(ViewDonHang.Phieu, forceNew);
            switch (err)
            {
                case 0:
                    //DebugMsg1.AddMessage("Không tạo phiếu mới!");
                    break;
                case 1:
                    DebugMsg1.AddMessage("Phiếu mới được tạo!");
                    break;
                case -1:
                    DebugMsg1.AddMessage("Lỗi tạo phiếu: Chưa chọn đơn hàng!");
                    break;
                case -2:
                    DebugMsg1.AddMessage("Lỗi tạo phiếu: Chưa chọn công thức!");
                    break;
            }
            return err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttme"></param>
        /// <param name="flags"></param>
        /// <param name="loaitp">0 - Xi; 1 - CL; 2 - Nước; 3 - PG</param>
        /// <returns></returns>
        private async Task ChotMe(int ttme, int flags, int loaitp)
        {
            if (ttme < 1)
            {
                return;
            }

            if (_daAnNutDung) 
                flags = 2;

            int err = TaoPhieu(false);
            System.Diagnostics.Debug.WriteLine($"Chốt mẻ theo {loaitp} -> tạo phiếu -> {err}");
            if (err >= 0)
            {
                DebugMsg1.AddMessage($"Số mẻ hiện tại: {_vm.DsMe.Count} ?? {ttme}");

                DHMeDO me;
                if (_vm.DsMe.Count < ttme)
                {
                    me = new()
                    {
                        STT = ttme,
                        M3Tron = _m3me,
                        Flags = flags | (_is_sim0 ? 4 : 0)
                    };
                    TPPhuGia.FillMeByCP(me);
                    DebugMsg1.AddMessage($"Tạo mẻ {ttme}");
                }
                else
                {
                    me = _vm.DsMe[ttme - 1];
                    me.Flags = flags | (_is_sim0 ? 4 : 0);
                    DebugMsg1.AddMessage($"Lấy lại mẻ {ttme}\r\n{me.GetChotInfo()}");
                }

                bool duLieuMoi = _daAnNutDung;
                duLieuMoi |= TPCotLieu.FillMe(me);
                duLieuMoi |= TPXiMang.FillMe(me);
                duLieuMoi |= TPNuoc.FillMe(me);
                duLieuMoi |= TPPhuGia.FillMe(me);
                // TODO: điền dữ liệu phụ gia cân ngoài khi chốt xi măng!

                if (duLieuMoi)
                {
                    await _db.PM_Activites_SaveAsync(4, 3, $"Kiểm tra chốt mẻ Id={me.Id} theo tp {loaitp}");
                    err = await _vm.SaveMe(me);
                    switch (err)
                    {
                        case 1:
                            DebugMsg1.AddMessage($"Lưu mẻ:\r\n{me.GetDesc(ModelHeThong.SoCLReal, ModelHeThong.SoXMReal, ModelHeThong.SoPGReal)}");
                            await _db.PM_Activites_SaveAsync(4, 1, "Chốt " + me.GetChotInfo());
                            break;
                        case 0:
                            DebugMsg1.AddMessage($"Không lưu mẻ vì ko tạo được phiếu! ERROR");
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt {loaitp}: ko tạo được phiếu");
                            break;
                        case -1:
                            DebugMsg1.AddMessage($"Không lưu mẻ vì không có tp nào có kl > {_vm.KLMinLuuMe}!");
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt {loaitp}: không có kl");
                            break;
                        case -2:
                            DebugMsg1.AddMessage($"Không lưu mẻ vì đang nạp liệu (đã lưu!)");
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt: đang nạp liệu");
                            break;
                        case -3:
                            DebugMsg1.AddMessage($"Không lưu mẻ vì không có kl trên cân (cân cl & cân xi < {_vm.KLMinLuuMe}");
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt: ko kl trên cân cl & xi");
                            break;
                        case -4:
                            DebugMsg1.AddMessage($"Không lưu mẻ {me.STT} == {ttme} " + me.GetChotInfo());
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt {me.GetChotInfo()}");
                            break;
                        case -5:
                            if ((flags & 2) == 2)
                                DebugMsg1.AddMessage("Không lưu mẻ vì mẻ đã được lưu: dừng chạy");
                            else
                                DebugMsg1.AddMessage("Không lưu mẻ vì mẻ đã được lưu: chốt mẻ");
                            await _db.PM_Activites_SaveAsync(4, 2, $"Ko chốt: Error = -5");
                            break;
                    }
                }
                else
                {
                    DebugMsg1.AddMessage("Không lưu phiếu vì ko có dữ liệu mới");
                }
            }
            else
            {
                DebugMsg1.AddMessage("Lỗi tạo phiếu");
            }
            StartSync();
        }
        #endregion

        #region Sync
        //private SyncAgent _sync = new();
        /// <summary>
        /// Server Sync Agent
        /// </summary>
        private ServerComm _ssAgent = new();
        //private OnlineMonitorClient _client = new();

        private async void InitServerSync()
        {
            var s = DbRepository.Instance.Settings;
            string? srvip = s.GetValue("srv.ip");
            string? srvdb = s.GetValue("srv.db");
            string? srvuser = s.GetValue("srv.user");
            string? srvpw = s.GetValue("srv.pw");

            if (!_ssAgent.IsLocalOk)
            {
                await _ssAgent.InitLocal();
                PnlServer.Visibility = _ssAgent.IsLocalOk ? Visibility.Visible : Visibility.Collapsed;

                //_client.Init();
            }

            if (_ssAgent.IsLocalOk)
            {
                LblSrvStatus.Text = "Test connection";
                if (string.IsNullOrEmpty(srvip) || string.IsNullOrEmpty(srvuser) || string.IsNullOrEmpty(srvpw))
                {
                    LblSrvStatus.Text = "Invalid";
                    LblSrvStatus.Foreground = Brushes.Red;
                    return;
                }
                await _ssAgent.Connect(srvip, srvuser, srvpw);

                if (_ssAgent.IsServerOk)
                {
                    LblSrvStatus.Foreground = Brushes.DarkBlue;

                    string? tramten = s.GetValue("srv.tram.id");
                    string? tramma = s.GetValue("srv.tram.ma");
            
                    await _ssAgent.GetSourceId(tramten, tramma);

                    if (_ssAgent.FactoryId > 0)
                    {
                        LblSrvStatus.Foreground = Brushes.Black;
                        StartSync();

                        //_client.Start(_sync.SourceId);
                    }
                    else
                    {
                        LblSrvStatus.Text = "Invalid";
                        LblSrvStatus.Foreground = Brushes.Red;

                        //_client.Stop();
                    }
                }
                else
                {
                    LblSrvStatus.Text = "Disconnected";
                    LblSrvStatus.Foreground = Brushes.Red;

            //        //_client.Stop();
                }
            }
        }

        private async void StartSync()
        {
            LblSrvStatus.Text = "Synchronizing...";
            await _ssAgent.StartSync();
            LblSrvStatus.Text = "Idle";
        }

        private void UpdateOnlineMonitor()
        {
            // Xe skip
            CtrlXeSkip.ZDT0 = _tramtron.XeSkipDT0.GetBool();
            CtrlXeSkip.ZDT1 = _tramtron.XeSkipDT1.GetBool();
            CtrlXeSkip.ZDT2 = _tramtron.XeSkipDT2.GetBool();
            CtrlXeSkip.ZIsDown = _tramtron.XeSkipDown.GetBool();
            CtrlXeSkip.ZIsUp = _tramtron.XeSkipUp.GetBool();
            CtrlXeSkip.ZEMC = _tramtron.XeSkipEMC.GetBool();
            CtrlXeSkip.TGXaCLEt = (int)_tramtron.XeSkipTGXaCLDT2.Value;

            // M3 đặt và số mẻ
            double m3dat = _tramtron.MeM3Dat.Value;
            int medat = (int)_tramtron.MeSoMe.Value;
            TxtTheTich.Text = m3dat.ToString("F1");
            _m3me = medat > 0 ? m3dat / medat : 0;
            TxtSoMe.Text = medat.ToString();
            TPCotLieu.MeDat = medat;
            TPNuoc.MeDat = medat;
            TPXiMang.MeDat = medat;
            TPPhuGia.MeDat = medat;

            //_client.TramKetNoiPLC = _tramtron.CommState == CommStates.Opened;
            //_client.TramCoiTron = _tramtron.MixerRunning.GetBool();
            //_client.TramDangTron = _tramtron.SysRunning.GetBool();

        }
        #endregion

        #region Debug
        private void BtDebuggerConfig_Click(object sender, RoutedEventArgs e)
        {
            Debugger.WndDebug wnd = new()
            {
                Owner = this,
                TramTron = _tramtron,
                Observer = _debugObserver,
            };
            _debugObserver.Stop();
            BtDebuggerStart.Content = "Start";
            wnd.Show();
        }
        private void MniDebug_Click(object sender, RoutedEventArgs e)
        {
            if (PnlDebug.Visibility != Visibility.Visible)
            {
                PnlDebug.Visibility = Visibility.Visible;
            }
            else
            {
                _debugObserver.Stop();
                PnlDebug.Visibility = Visibility.Collapsed;
            }
        }

        private void BtDebuggerStart_Click(object sender, RoutedEventArgs e)
        {
            if (_debugObserver.CanObserve)
            {
                _debugObserver.Stop();
                BtDebuggerStart.Content = "Start";
            }
            else
            {
                _debugObserver.Start();
                BtDebuggerStart.Content = "Stop";
            }
        }

        private void BtDebuggerExport_Click(object sender, RoutedEventArgs e)
        {
            _debugObserver.ExportText();
        }

        private void MniDebugInfo_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new();
            sb.Append("Thời gian trễ chốt khối lượng:\n");
            sb.Append($"Cân CL1  {TPCotLieu.WsTotalCL1.ChotKLTre} s\n");
            sb.Append($"Cân CL2  {TPCotLieu.WsTotalCL2.ChotKLTre} s\n");
            sb.Append($"Cân CL3  {TPCotLieu.WsTotalCL3.ChotKLTre} s\n");
            sb.Append($"Cân Xi1  {TPXiMang.WSCements1.ChotKLTre} s\n");
            sb.Append($"Cân Xi2  {TPXiMang.WSCements2.ChotKLTre} s\n");
            sb.Append($"Cân Nước {TPNuoc.WSWater.ChotKLTre} s\n");
            sb.Append($"Cân PG   {TPPhuGia.WScale1.ChotKLTre} s\n");
            DebugMsg1.AddMessage(sb.ToString());
        }
        #endregion
    }
}