using FastReport;
using ManualPrintDocket.CSDL;
using ManualPrintDocket.ViewModel;
using NMPrinting;
using System.Globalization;
using System.IO;
using System.Windows;

namespace ManualPrintDocket.Printing
{
    public class XuLyInPhieu
    {
        #region Singleton
        private XuLyInPhieu()
        {
            _vnCulture.NumberFormat.NumberDecimalSeparator = ",";
            _vnCulture.NumberFormat.NumberGroupSeparator = "."; // nếu muốn ngăn cách hàng nghìn
        }
        private static XuLyInPhieu? _instance;

        public static XuLyInPhieu Instance => _instance ??= new XuLyInPhieu();
        #endregion

        private DbRepository _r = DbRepository.Instance;
        public string? ReportTemplate { get; private set; }
        private readonly Report _report = new();
        // Tạo CultureInfo dựa trên "en-US" nhưng đổi dấu thập phân thành ','
        private CultureInfo _vnCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

        public bool LoadTemplate(string templateFile)
        {
            try
            {
                ReportTemplate = _r.ReportsPath + templateFile;
                _report.Load(ReportTemplate);
                _report.Dictionary.Connections.Clear();
                return true;
            }
            catch (Exception ex)
            {
                ReportTemplate = null;
                MessageBox.Show(ex.Message, "Load docket template");
            }
            return false;
        }

        public async Task<bool> Print(PhieuTronBeTongVM ph, bool showDialog = false, int solan = 1)
        {
            try
            {
                _report.SetParameterValue("Title.CTy.Ten", "");
                _report.SetParameterValue("Title.CTy.DiaChi", "");
                _report.SetParameterValue("Title.CTy.SDT1", "");
                _report.SetParameterValue("Title.CTy.SDT2", "");

                _report.SetParameterValue("Phieu.SoPhieu", ph.SoPhieu);
                _report.SetParameterValue("Phieu.STT", ph.SoPhieu);

                //_report.SetParameterValue("Don.Ma", "");
                //_report.SetParameterValue("Don.M3", "");

                //_report.SetParameterValue("KH.Ma", "");
                _report.SetParameterValue("KH.Ten", ph.KhachHangTen);
                //_report.SetParameterValue("KH.DiaChi", "");
                //_report.SetParameterValue("KH.SDT", "");
                _report.SetParameterValue("DuAn.DA", ph.DuAnTen);
                _report.SetParameterValue("DuAn.HM", ph.DuAnTen);
                _report.SetParameterValue("DuAn.DiaChi", ph.DuAnDiaChi);
                _report.SetParameterValue("Don.TichLuy", ph.TongM3Tron);

                _report.SetParameterValue("Xe.BSX", ph.XeBienSo);
                _report.SetParameterValue("Xe.LaiXe", ph.XeLaiXe);

                _report.SetParameterValue("CapPhoi.Ten", "");
                _report.SetParameterValue("CapPhoi.Mac", ph.BeTongMac);
                _report.SetParameterValue("CapPhoi.Slump", ph.BeTongDoSut);
                _report.SetParameterValue("CapPhoi.AggMax", ph.BeTongDkCotLieuMax);

                _report.SetParameterValue("Phieu.M3", ph.M3Tron);
                _report.SetParameterValue("Phieu.KL", ph.TongM3Tron);
                _report.SetParameterValue("Phieu.TongKL", "");

                _report.SetParameterValue("Phieu.NgayXuatTram", $"Ngày {ph.TGHT.Day} tháng {ph.TGHT.Month} năm {ph.TGHT.Year}");
                _report.SetParameterValue("Phieu.GioXuatTram", $"{ph.TGHT.Hour} h {ph.TGHT.Minute}");
                _report.SetParameterValue("Phieu.NgayXuatTramShort", $"{ph.TGHT.Day}/{ph.TGHT.Month}/{ph.TGHT.Year}");
                _report.SetParameterValue("Phieu.GioXuatTramShort", $"{ph.TGHT.Hour}:{ph.TGHT.Minute}");

                await _report.PrepareAsync();
                using var ms = GetReportAsPdfStreamAsync(_report);
                PrintUtils.PrintPdfMemoryStream(ms, showDialog, solan);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi in phiếu: {0}", ex.Message);
            }
            return false;
        }

        private MemoryStream GetReportAsPdfStreamAsync(Report report)
        {
            FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new();

            MemoryStream ms = new();

            // Configure export if needed
            pdfExport.Export(report, ms);
            ms.Position = 0; // Reset stream position

            return ms;
        }

    }
}
