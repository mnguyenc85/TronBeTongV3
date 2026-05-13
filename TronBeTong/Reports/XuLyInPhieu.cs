using System;
using System.Data;
using System.Globalization;
using System.IO;
using FastReport;
using NMPrinting;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.ViewModel.DonHang;
using TronBeTongV3.Data.ViewModel.ThongKe;

namespace TronBeTongV3.Reports
{
    public class XuLyInPhieu
    {
        #region Singleton
        // Tạo CultureInfo dựa trên "en-US" nhưng đổi dấu thập phân thành ','
        private CultureInfo _vnCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        private static XuLyInPhieu? _instance;
        public static XuLyInPhieu Instance => _instance ??= new XuLyInPhieu();
        private XuLyInPhieu() {
            _vnCulture.NumberFormat.NumberDecimalSeparator = ",";
            _vnCulture.NumberFormat.NumberGroupSeparator = "."; // nếu muốn ngăn cách hàng nghìn
        }
        #endregion

        private DbRepository _r = DbRepository.Instance;
        public string? ReportTemplate { get; private set; }
        private readonly Report _reportTong = new();
        public string? ReportTemplateChiTiet { get; private set; }
        private readonly Report _reportChiTiet = new();
        //private ReportPreviewWnd? _previewWnd;

        public void Close()
        {
            //if (_previewWnd != null) { _previewWnd.CloseWindowProgrammatically(); }
        }

        public void LoadTemplate()
        {
            ReportTemplate = null;
            try
            {
                var t = _r.Settings.GetValue("in.phieu.template");

                if (t != null)
                {
                    string path = _r.ReportsPath + t;
                    if (System.IO.File.Exists(path))
                    {
                        _reportTong.Load(path);
                        _reportTong.Dictionary.Connections.Clear();
                        ReportTemplate = path;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            ReportTemplateChiTiet = null;
            try
            {
                var t = _r.Settings.GetValue("in.phieu.template.chitiet");

                if (t != null)
                {
                    string path = _r.ReportsPath + t;
                    if (System.IO.File.Exists(path))
                    {
                        _reportChiTiet.Load(path);
                        _reportChiTiet.Dictionary.Connections.Clear();
                        ReportTemplateChiTiet = path;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        [Obsolete]
        public async Task<bool> Print(DHPhieuVM ph, TKMeInVM siloma, TKMeInVM silotongkl, HackValue? ghide = null, bool showDialog = false, int solan = 1)
        {
            var report = _reportTong;

            try
            {
                report.SetParameterValue("Title.CTy.Ten", _r.Settings.GetValue("in.phieu.cty.ten"));
                report.SetParameterValue("Title.CTy.DiaChi", _r.Settings.GetValue("in.phieu.cty.diachi"));
                report.SetParameterValue("Title.CTy.SDT1", _r.Settings.GetValue("in.phieu.cty.sdt1"));
                report.SetParameterValue("Title.CTy.SDT2", _r.Settings.GetValue("in.phieu.cty.sdt2"));

                report.SetParameterValue("Phieu.SoPhieu", ph.SoPhieu);
                report.SetParameterValue("Phieu.STT", ph.STT);
                report.SetParameterValue("Phieu.KepChi", ph.KepChi);

                if (ph.DonHang != null)
                {
                    report.SetParameterValue("Don.Ma", ph.DonHang.Ma);                    
                    report.SetParameterValue("Don.M3", ph.DonHang.TheTichDH);

                    report.SetParameterValue("KH.Ma", ph.DonHang.KhachHang?.Ma);
                    report.SetParameterValue("KH.Ten", ph.DonHang.KhachHang?.Ten);
                    report.SetParameterValue("KH.DiaChi", ph.DonHang.KhachHang?.DiaChi);
                    report.SetParameterValue("KH.SDT", ph.DonHang.KhachHang?.Sdt);
                    if (ph.DonHang.DuAn != null)
                    {
                        string hm = "";
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.DuAn)) hm += ph.DonHang.DuAn.DuAn;
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.CongTrinh))
                        {
                            if (hm.Length > 0) hm += " - ";
                            hm += ph.DonHang.DuAn.CongTrinh;
                        }
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.HangMuc))
                        {
                            if (hm.Length > 0) hm += " - ";
                            hm += ph.DonHang.DuAn.HangMuc;
                        }
                        report.SetParameterValue("DuAn.HM", hm);
                        report.SetParameterValue("DuAn.DiaChi", ph.DonHang.DuAn.DiaChi);
                    }
                    report.SetParameterValue("Don.TichLuy", string.Format(_vnCulture, "{0:N1}", ph.DonHang.TheTichHT));
                }
                if (ph.Xe != null)
                    report.SetParameterValue("Xe.BSX", ph.Xe.BSX);
                if (ph.LaiXe != null)
                    report.SetParameterValue("Xe.LaiXe", ph.LaiXe.Ten);

                report.SetParameterValue("CapPhoi.Ten", ph.CongThuc?.Ma);
                report.SetParameterValue("CapPhoi.Mac", ph.CongThuc?.Mac);
                report.SetParameterValue("CapPhoi.Slump", ph.CongThuc?.Slump);
                report.SetParameterValue("CapPhoi.AggMax", ph.CongThuc?.KTHat);

                report.SetParameterValue("Phieu.M3", ph.TheTichHT.ToString("N1", _vnCulture));
                report.SetParameterValue("Phieu.KL", ph.KLHT.ToString("N0", _vnCulture));
                report.SetParameterValue("Phieu.TongKL", string.Format(_vnCulture, "{0:N1}/{1:N1}", ph.TheTichTichLuy, ph.DonHang?.TheTichDH));

                //_report.SetParameterValue("CapPhoi.CL1", siloma.KLCL0);
                //_report.SetParameterValue("CapPhoi.CL2", siloma.KLCL1);
                //_report.SetParameterValue("CapPhoi.CL3", siloma.KLCL2);
                //_report.SetParameterValue("CapPhoi.CL4", siloma.KLCL3);
                //_report.SetParameterValue("CapPhoi.Xi1", siloma.KLXi0);
                //_report.SetParameterValue("KLTP.CL1", silotongkl.KLCL0);
                //_report.SetParameterValue("KLTP.CL2", silotongkl.KLCL1);
                //_report.SetParameterValue("KLTP.CL3", silotongkl.KLCL2);
                //_report.SetParameterValue("KLTP.CL4", silotongkl.KLCL3);
                //_report.SetParameterValue("KLTP.Xi1", silotongkl.KLXi0);

                if (ph.TGHT != null)
                {
                    report.SetParameterValue("Phieu.NgayXuatTram", $"Ngày {ph.TGHT?.Day} tháng {ph.TGHT?.Month} năm {ph.TGHT?.Year}");
                    report.SetParameterValue("Phieu.GioXuatTram", $"{ph.TGHT?.Hour} h {ph.TGHT?.Minute}");
                    report.SetParameterValue("Phieu.NgayXuatTramShort", $"{ph.TGHT?.Day}/{ph.TGHT?.Month}/{ph.TGHT?.Year}");
                    report.SetParameterValue("Phieu.GioXuatTramShort", $"{ph.TGHT?.Hour}:{ph.TGHT?.Minute}");
                }

                if (ghide != null)
                {
                    report.SetParameterValue("Phieu.SoPhieu", ghide.SoPhieu);
                    report.SetParameterValue("Phieu.STT", ghide.SoPhieu);
                    report.SetParameterValue("Phieu.NgayXuatTram", ghide.NgayTron);
                    report.SetParameterValue("Phieu.GioXuatTram", ghide.TGRoiTram);
                    report.SetParameterValue("Phieu.NgayXuatTramShort", ghide.NgayTron);
                    report.SetParameterValue("Phieu.GioXuatTramShort", ghide.TGRoiTram);

                    report.SetParameterValue("KH.Ma", ghide.KhachHang);
                    report.SetParameterValue("KH.Ten", ghide.KhachHang);
                    report.SetParameterValue("DuAn.DA", ghide.DuAn);
                    report.SetParameterValue("DuAn.CT", ghide.DuAn);
                    report.SetParameterValue("DuAn.HM", ghide.DuAn);
                    report.SetParameterValue("DuAn.DiaChi", ghide.DuAnDiaChi);

                    report.SetParameterValue("Xe.BSX", ghide.BienSoXe);
                    report.SetParameterValue("Xe.LaiXe", ghide.LaiXe);

                    //_report.SetParameterValue("CapPhoi.Ten", ph.CongThuc?.Ma);
                    report.SetParameterValue("CapPhoi.Mac", ghide.BeTongMac);
                    report.SetParameterValue("CapPhoi.Slump", ghide.BeTongSlump);
                    report.SetParameterValue("CapPhoi.AggMax", ghide.BeTongMaxAgg);
                    report.SetParameterValue("Phieu.MaSo", ghide.MaSo);

                    report.SetParameterValue("Phieu.M3", ghide.M3);
                    report.SetParameterValue("Don.TichLuy", ghide.TichLuy);
                }

                await report.PrepareAsync();
                using var ms = FastReportPrintUtils.GetReportAsPdfStreamAsync(report);
                PrintUtils.PrintPdfMemoryStream(ms, showDialog, solan);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> Print(DHPhieuVM ph, List<DHMeDO> dsme, bool inChiTiet = false, HackValue? ghide = null, bool showDialog = false, int solan = 1)
        {
            List<TKMeInVM> dstkme = [];

            #region Tính kl thành phần
            // Mã thành phần
            TKMeInVM siloma = new() { Flags = 64 };
            if (ph.CongThuc != null) siloma.TenNLFromDHCongThuc(ph.CongThuc);

            dstkme.Add(siloma);

            // KL từng mẻ
            int n = 0;
            TKMeInVM me;
            foreach (var i in dsme)
            {
                me = new()
                {
                    STT = (++n).ToString(),
                    Flags = i.Flags,
                };
                me.FromDHMeDO(i);

                dstkme.Add(me);
            }

            // Tổng KL
            ph.KiemTraThongTin(dsme);
            TKMeInVM silotongkl = new() { Flags = 128 };
            silotongkl.FromPhieuTongKL(ph);

            dstkme.Add(silotongkl);
            #endregion

            return await Print(ph, dstkme, inChiTiet, ghide, showDialog, solan);
        }
        public async Task<bool> Print(DHPhieuVM ph, List<TKMeInVM> dsme, bool inChiTiet, HackValue ? ghide, bool showDialog = false, int solan = 1)
        {
            var report = inChiTiet ? _reportChiTiet : _reportTong;

            //var siloma = dsme.Count > 0 ? dsme[0] : null;
            //var silotongkl = dsme.Count > 1? dsme[dsme.Count - 1] : null;

            try
            {
                report.SetParameterValue("Title.CTy.Ten", _r.Settings.GetValue("in.phieu.cty.ten"));
                report.SetParameterValue("Title.CTy.DiaChi", _r.Settings.GetValue("in.phieu.cty.diachi"));
                report.SetParameterValue("Title.CTy.SDT1", _r.Settings.GetValue("in.phieu.cty.sdt1"));
                report.SetParameterValue("Title.CTy.SDT2", _r.Settings.GetValue("in.phieu.cty.sdt2"));

                report.SetParameterValue("Phieu.SoPhieu", ph.SoPhieu);
                report.SetParameterValue("Phieu.STT", ph.STT);
                report.SetParameterValue("Phieu.KepChi", ph.KepChi);

                if (ph.DonHang != null)
                {
                    report.SetParameterValue("Don.Ma", ph.DonHang.Ma);
                    report.SetParameterValue("Don.M3", ph.DonHang.TheTichDH);

                    report.SetParameterValue("KH.Ma", ph.DonHang.KhachHang?.Ma);
                    report.SetParameterValue("KH.Ten", ph.DonHang.KhachHang?.Ten);
                    report.SetParameterValue("KH.DiaChi", ph.DonHang.KhachHang?.DiaChi);
                    report.SetParameterValue("KH.SDT", ph.DonHang.KhachHang?.Sdt);
                    if (ph.DonHang.DuAn != null)
                    {
                        string hm = "";
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.DuAn)) hm += ph.DonHang.DuAn.DuAn;
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.CongTrinh))
                        {
                            if (hm.Length > 0) hm += " - ";
                            hm += ph.DonHang.DuAn.CongTrinh;
                        }
                        if (!string.IsNullOrEmpty(ph.DonHang.DuAn.HangMuc))
                        {
                            if (hm.Length > 0) hm += " - ";
                            hm += ph.DonHang.DuAn.HangMuc;
                        }
                        report.SetParameterValue("DuAn.HM", hm);
                        report.SetParameterValue("DuAn.DiaChi", ph.DonHang.DuAn.DiaChi);
                    }
                    report.SetParameterValue("Don.TichLuy", string.Format(_vnCulture, "{0:N1}", ph.DonHang.TheTichHT));
                }
                if (ph.Xe != null)
                    report.SetParameterValue("Xe.BSX", ph.Xe.BSX);
                if (ph.LaiXe != null)
                    report.SetParameterValue("Xe.LaiXe", ph.LaiXe.Ten);

                report.SetParameterValue("CapPhoi.Ten", ph.CongThuc?.Ma);
                report.SetParameterValue("CapPhoi.Mac", ph.CongThuc?.Mac);
                report.SetParameterValue("CapPhoi.Slump", ph.CongThuc?.Slump);
                report.SetParameterValue("CapPhoi.AggMax", ph.CongThuc?.KTHat);

                report.SetParameterValue("Phieu.M3", ph.TheTichHT.ToString("N1", _vnCulture));
                report.SetParameterValue("Phieu.KL", ph.KLHT.ToString("N0", _vnCulture));
                report.SetParameterValue("Phieu.TongKL", string.Format(_vnCulture, "{0:N1}/{1:N1}", ph.TheTichTichLuy, ph.DonHang?.TheTichDH));
                //_report.SetParameterValue("Don.TichLuy", string.Format(_vnCulture, "{0:N2}", ph.TheTichTichLuy));

                if (ph.TGHT != null)
                {
                    report.SetParameterValue("Phieu.NgayXuatTram", $"Ngày {ph.TGHT?.Day} tháng {ph.TGHT?.Month} năm {ph.TGHT?.Year}");
                    report.SetParameterValue("Phieu.GioXuatTram", $"{ph.TGHT?.Hour} h {ph.TGHT?.Minute}");
                    report.SetParameterValue("Phieu.NgayXuatTramShort", $"{ph.TGHT?.Day}/{ph.TGHT?.Month}/{ph.TGHT?.Year}");
                    report.SetParameterValue("Phieu.GioXuatTramShort", $"{ph.TGHT?.Hour}:{ph.TGHT?.Minute}");
                }

                if (inChiTiet)
                {
                    // Gửi dữ liệu mẻ
                    var dt = FastReportPrintUtils.ConvertToDataTable<TKMeInVM>(dsme);
                    dt.TableName = "TKMeInVM";
                    DataSet ds = new DataSet("dataset");
                    ds.Tables.Add(dt);

                    report.RegisterData(ds, "dataset", true);
                    var dataSource = report.GetDataSource("TKMeInVM");

                    for (int i = 1; i < 10; i++)
                    {
                        var dataBand = report.FindObject($"Data{i}") as DataBand;
                        if (dataBand == null) break;
                        dataBand.DataSource = dataSource;
                    }
                }

                if (ghide != null)
                {
                    report.SetParameterValue("Phieu.SoPhieu", ghide.SoPhieu);
                    report.SetParameterValue("Phieu.STT", ghide.SoPhieu);
                    report.SetParameterValue("Phieu.NgayXuatTram", ghide.NgayTron);
                    report.SetParameterValue("Phieu.GioXuatTram", ghide.TGRoiTram);
                    report.SetParameterValue("Phieu.NgayXuatTramShort", ghide.NgayTron);
                    report.SetParameterValue("Phieu.GioXuatTramShort", ghide.TGRoiTram);

                    report.SetParameterValue("KH.Ma", ghide.KhachHang);
                    report.SetParameterValue("KH.Ten", ghide.KhachHang);
                    report.SetParameterValue("DuAn.DA", ghide.DuAn);
                    report.SetParameterValue("DuAn.CT", ghide.DuAn);
                    report.SetParameterValue("DuAn.HM", ghide.DuAn);
                    report.SetParameterValue("DuAn.DiaChi", ghide.DuAnDiaChi);

                    report.SetParameterValue("Xe.BSX", ghide.BienSoXe);
                    report.SetParameterValue("Xe.LaiXe", ghide.LaiXe);

                    //_report.SetParameterValue("CapPhoi.Ten", ph.CongThuc?.Ma);
                    report.SetParameterValue("CapPhoi.Mac", ghide.BeTongMac);
                    report.SetParameterValue("CapPhoi.Slump", ghide.BeTongSlump);
                    report.SetParameterValue("CapPhoi.AggMax", ghide.BeTongMaxAgg);
                    report.SetParameterValue("Phieu.MaSo", ghide.MaSo);

                    report.SetParameterValue("Phieu.M3", ghide.M3);
                    report.SetParameterValue("Don.TichLuy", ghide.TichLuy);
                }

                await report.PrepareAsync();
                using var ms = FastReportPrintUtils.GetReportAsPdfStreamAsync(report);
                PrintUtils.PrintPdfMemoryStream(ms, showDialog, solan);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
