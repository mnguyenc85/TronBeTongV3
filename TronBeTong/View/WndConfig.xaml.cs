using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using TronBeTongV3.CSDL;
using TronBeTongV3.CSDL.Server;
using TronBeTongV3.Data.ViewModel.Config;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndConfig.xaml
    /// </summary>
    public partial class WndConfig : Window
    {
        private bool _is_local_connect = false;
        private bool _has_test_server_connect = false;
        private string? _tram_ma;
        public List<DbSrc> DbSrcs { get; set; } = new();

        public ObservableCollection<NetworkInterface> Networks { get; set; } = [];

        public int Privilege { get; set; }

        public WndConfig()
        {
            InitializeComponent();
            DataContext = this;

            DbSrcs.Add(new DbSrc("0") { Name = "TronBeTong (MariaDb-NCM)" });
            DbSrcs.Add(new DbSrc("1") { Name = "TronBeTong (MariaDb-13306)" });
            DbSrcs.Add(new DbSrc("2") { Name = "TronBeTong (MySQL)" });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private async void BtTestConn_Click(object sender, RoutedEventArgs e)
        {
            var db = DbBridge.Instance;
            try
            {
                db.DbSrcName = CboDbSrcs.SelectedValue as string;
                db.Init();
                await db.CreateDatabaseAsync();
                await db.CreateTableAsync();

                MessageBox.Show("Kết nối thành công!", "CSDL");

                _is_local_connect = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Không kết nối được CSDL!\n{0}", ex.Message), "CSDL");
                _is_local_connect = false;
                CboDbSrcs.SelectedValue = db.DbSrcName;
            }
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private async void LoadSettings()
        {
            var db = DbBridge.Instance;
            var r = DbRepository.Instance;
            var s = r.Settings;

            ListAllTemplates(r.ReportsPath);

            CboDbSrcs.SelectedItem = DbSrcs.Where(x => x.Id == db.DbSrcName).FirstOrDefault();

            int commtype = r.Settings.GetIntValue("plc.comm");
            OptS71200Ethernet.IsChecked = commtype == 1;
            OptS7200KEPServer.IsChecked = commtype == 2;
            TxtPLCAddr.Text = r.Settings.GetValue("plc.s71200.addr");
            TxtLocalIp.Text = r.Settings.GetValue("plc.s71200.localip");
            TxtKSrvPath.Text = r.Settings.GetValue("plc.s7200.ksrv.path");

            CboMauPhieu.SelectedItem = s.GetValue("in.phieu.template");
            CboMauPhieuChiTiet.SelectedItem = s.GetValue("in.phieu.template.chitiet");
            CboMauThongKe.SelectedItem = s.GetValue("in.baocao.template");
            CboMauThongKeChiTiet.SelectedItem = s.GetValue("in.baocao.template.chitiet");

            TxtCTyTen.Text = s.GetValue("in.phieu.cty.ten");
            TxtCTyDiaChi.Text = s.GetValue("in.phieu.cty.diachi");
            TxtCTySDT1.Text = s.GetValue("in.phieu.cty.sdt1");
            TxtCTySDT2.Text = s.GetValue("in.phieu.cty.sdt2");
            TxtLogoPath.Text = s.GetValue("in.phieu.cty.logo");
            ChkAutoPrint.IsChecked = s.GetBoolValue("in.phieu.auto");
            TxtPrintTimes.Text = s.GetValue("in.phieu.auto.times");
            //try
            //{
            //    if (!string.IsNullOrEmpty(TxtLogoPath.Text))
            //        ImgLogo.Source = LoadImageNoLock(TxtLogoPath.Text);
            //}
            //catch { }

            ChkSimEn.IsChecked = s.GetBoolValue("sim.en");

            TxtTramId.Text = s.GetValue("srv.tram.id");
            TxtServerIP.Text = s.GetValue("srv.ip");
            TxtServerDb.Text = s.GetValue("srv.db");
            TxtServerUser.Text = s.GetValue("srv.user");
            txtServerPw.Password = s.GetValue("srv.pw");

            TxtXeSkipDT01.Text = s.GetValue("vanhanh.xeskip.t01");
            TxtXeSkipDT12.Text = s.GetValue("vanhanh.xeskip.t12");

            ChkShowDebug.IsChecked = s.GetBoolValue("debug.visible");
            ChkAutoCongThucDonHang.IsChecked = s.GetBoolValue("auto.congthuc.donhang");
            ChkPhieuAllowChangeM3.IsChecked = s.GetBoolValue("hack.phieu.m3");

            // Liệt kê danh sách Network interfaces
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface net in interfaces)
            {
                Networks.Add(net);
            }

            try
            {
                var sz = await db.GetDbSizeAsync();
                TxtDbSize.Text = Math.Round((double)sz / 1048576d, 3).ToString();
            }
            catch { }

            LoadChotKLSettings();
        }

        private async void SaveSettings()
        {
            var db = DbBridge.Instance;
            var r = DbRepository.Instance;
            var s = r.Settings;

            s.Update("in.phieu.template", CboMauPhieu.Text);
            s.Update("in.phieu.template.chitiet", CboMauPhieuChiTiet.Text);
            s.Update("in.baocao.template", CboMauThongKe.Text);
            s.Update("in.baocao.template.chitiet", CboMauThongKeChiTiet.Text);

            s.Update("in.phieu.cty.ten", TxtCTyTen.Text);
            s.Update("in.phieu.cty.diachi", TxtCTyDiaChi.Text);
            s.Update("in.phieu.cty.sdt1", TxtCTySDT1.Text);
            s.Update("in.phieu.cty.sdt2", TxtCTySDT2.Text);
            s.Update("in.phieu.cty.logo", TxtLogoPath.Text);
            s.UpdateBool("in.phieu.auto", ChkAutoPrint.IsChecked == true);
            s.UpdateDouble("in.phieu.auto.times", TxtPrintTimes.Text);

            s.Update("vanhanh.xeskip.t01", TxtXeSkipDT01.Text);
            s.Update("vanhanh.xeskip.t12", TxtXeSkipDT12.Text);

            s.UpdateBool("debug.visible", ChkShowDebug.IsChecked == true);
            s.UpdateBool("auto.congthuc.donhang", ChkAutoCongThucDonHang.IsChecked == true);

            if (Privilege == 255)
            {
                s.Update("plc.s71200.addr", TxtPLCAddr.Text);
                s.Update("plc.s71200.localip", TxtLocalIp.Text);
                int commtype = 0;
                if (OptS71200Ethernet.IsChecked == true) commtype = 1;
                else if (OptS7200KEPServer.IsChecked == true) commtype = 2;
                s.UpdateDouble("plc.comm", commtype);
                s.Update("plc.s7200.ksrv.path", TxtKSrvPath.Text);

                s.UpdateBool("sim.en", ChkSimEn.IsChecked == true);

                if (_has_test_server_connect)
                {
                    s.Update("srv.tram.id", TxtTramId.Text);
                    s.Update("srv.tram.ma", _tram_ma ??= "");
                    s.Update("srv.ip", TxtServerIP.Text);
                    s.Update("srv.db", TxtServerDb.Text);
                    s.Update("srv.user", TxtServerUser.Text);
                    s.Update("srv.pw", txtServerPw.Password);
                }

                s.UpdateBool("hack.phieu.m3", ChkPhieuAllowChangeM3.IsChecked == true);

                SaveChotKLSettings(s);
            }

            try
            {
                if (_is_local_connect) db.Save();
                await db.SaveSettingsAsync(r.Settings);
            }
            catch { }
        }

        private void BtSimulationSetPass_Click(object sender, RoutedEventArgs e)
        {
            WndChangePass wnd = new WndChangePass() { Owner = this };
            wnd.ShowDialog();
        }

        private void BtPlcExportTags_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            Clipboard.SetText(sb.ToString());
        }

        private void TxtLogoPath_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) { 
                try
                {
                    ImgLogo.Source = LoadImageNoLock(TxtLogoPath.Text);
                }
                catch { }
            }
        }

        public static BitmapSource LoadImageNoLock(string filePath)
        {
            // Đọc toàn bộ file vào mảng byte
            byte[] imageData = File.ReadAllBytes(filePath);

            using (var ms = new MemoryStream(imageData))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load hết vào RAM
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze(); // Cho phép dùng ở thread khác, tránh lỗi cross-thread
                return bitmap;
            }
        }

        private void CboNetInterface_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CboNetInterface.SelectedItem is NetworkInterface ni)
            {
                // Lấy thông tin IP của interface
                IPInterfaceProperties ipProps = ni.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork) // chỉ lấy IPv4
                    {
                        TxtLocalIp.Text = ip.Address.ToString();
                    }
                    //else if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
                    //{
                    //    Console.WriteLine("  IPv6: " + ip.Address);
                    //}
                }

            }
        }

        private async void BtTestServerConnection_Click(object sender, RoutedEventArgs e)
        {
            var r = DbRepository.Instance;
            var s = r.Settings;

            _has_test_server_connect = true;

            var sync = new SyncAgent();
            await sync.Init(TxtServerIP.Text, TxtServerDb.Text, TxtServerUser.Text, txtServerPw.Password);

            if (sync.IsLocalOk)
            {
                _tram_ma = s.GetValue("srv.tram.ma");
                _tram_ma ??= $"{DateTime.Now:yyyyMMddHHmmss}";
                await sync.InitServer(TxtTramId.Text, _tram_ma);
                if (sync.IsServerOk)
                {
                    MessageBox.Show("Kết nối thành công!", "Server");
                }
            }
        }

        public void SetPrivilege(int p)
        {
            Privilege = p;
            if (p != 255)
            {
                TabItemCSDL.Visibility = Visibility.Collapsed;
                TabItemPLC.Visibility = Visibility.Collapsed;
                //TabItemOperate.Visibility = Visibility.Collapsed;
                PnlOpSim.Visibility = Visibility.Collapsed;
                TabItemServer.Visibility = Visibility.Collapsed;
                TabItemSaveResults.Visibility = Visibility.Collapsed;

                TabMain.SelectedItem = TabItemPhieu;
                PnlHack.Visibility = Visibility.Collapsed;
            }
        }

        private void ListAllTemplates(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath, "*.frx", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    CboMauPhieu.Items.Add(filename);
                    CboMauPhieuChiTiet.Items.Add(filename);
                    CboMauThongKe.Items.Add(filename);
                    CboMauThongKeChiTiet.Items.Add(filename);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Thư mục không tồn tại!");
            }
        }

        private async void BtDelLog_Click(object sender, RoutedEventArgs e)
        {
            var db = DbBridge.Instance;
            await db.PM_Activites_DelAsync();
        }

        #region Tham số chốt khối lượng
        public ObservableCollection<ThamSoChotKLVM> ChotKLSettings { get; set; } = [];
        private void BtTSChotKLAdd_Click(object sender, RoutedEventArgs e)
        {
            ChotKLSettings.Add(new ThamSoChotKLVM());
        }

        private void SaveChotKLSettings(DbSettings s)
        {
            foreach (var item in ChotKLSettings)
            {
                if (item.GiaTri != null)
                    s.Update($"chotkl.ts.{item.TenCan}.{item.TenBien}", item.GiaTri);
            }
        }

        private void LoadChotKLSettings()
        {
            var r = DbRepository.Instance;
            foreach (var item in r.Settings.Data.Values)
            {
                try
                {
                    if (item.Name.StartsWith("chotkl.ts."))
                    {
                        string[] ss = item.Name.Split('.');
                        if (ss.Length > 3)
                        {
                            ChotKLSettings.Add(new ThamSoChotKLVM()
                            {
                                TenCan = ss[2],
                                TenBien = ss[3],
                                GiaTri = item.Value
                            });
                        }
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
