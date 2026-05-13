using FastReport;
using NMPrinting;
using System;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.ThongKe;

namespace TronBeTongV3.Reports
{
    public class XuLyInBaoCao
    {
        #region Singleton
        private CultureInfo _vnCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        private static XuLyInBaoCao? _instance;
        public static XuLyInBaoCao Instance => _instance ??= new XuLyInBaoCao();
        private XuLyInBaoCao()
        {
            _vnCulture.NumberFormat.NumberDecimalSeparator = ",";
            _vnCulture.NumberFormat.NumberGroupSeparator = "."; // nếu muốn ngăn cách hàng nghìn
        }
        #endregion

        private DbRepository _r = DbRepository.Instance;
        public string? ReportTemplate { get; private set; }
        private readonly Report _reportTong = new();
        public string? ReportTemplateChiTiet { get; private set; }
        private readonly Report _reportChiTiet = new();

        public void LoadTemplate()
        {
            ReportTemplate = null;
            try
            {
                var t = _r.Settings.GetValue("in.baocao.template");

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
                var t = _r.Settings.GetValue("in.baocao.template.chitiet");

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

        public async Task<bool> Print(DKThongKeDO dieukien, List<TKMeInDO> dsme, List<TKThanhPhanDO> uniqueMa, bool inChiTiet, bool showDialog = false)
        {
            var report = inChiTiet ? _reportChiTiet : _reportTong;
            System.Diagnostics.Debug.WriteLine($"In báo cáo theo {ReportTemplate}");

            try
            {
                report.SetParameterValue("Title.CTy.Ten", _r.Settings.GetValue("in.phieu.cty.ten"));
                report.SetParameterValue("Title.CTy.DiaChi", _r.Settings.GetValue("in.phieu.cty.diachi"));
                report.SetParameterValue("Title.CTy.SDT1", _r.Settings.GetValue("in.phieu.cty.sdt1"));
                report.SetParameterValue("Title.CTy.SDT2", _r.Settings.GetValue("in.phieu.cty.sdt2"));
                
                report.SetParameterValue("DieuKien.MaDon", dieukien.MaDon);
                report.SetParameterValue("DieuKien.KhachHang", dieukien.KhachHang);
                report.SetParameterValue("DieuKien.CongTrinh", dieukien.CongTrinh);
                report.SetParameterValue("DieuKien.SoPhieu", dieukien.SoPhieu);
                report.SetParameterValue("DieuKien.BienSoXe", dieukien.BienSoXe);
                report.SetParameterValue("DieuKien.LaiXe", dieukien.LaiXe);
                report.SetParameterValue("DieuKien.TGBD", dieukien.TGBD);
                report.SetParameterValue("DieuKien.TGKT", dieukien.TGKT);
                report.SetParameterValue("DieuKien.MeTuDong", dieukien.MeTuDong);
                report.SetParameterValue("DieuKien.MeDungTay", dieukien.MeDungTay);
                report.SetParameterValue("DieuKien.MeMoPhong", dieukien.MeMoPhong);

                report.SetParameterValue("Khac.NgayIn.Vn", DateTime.Now.ToString("dd/MM/yyyy"));

                var tongkl = dsme.Find(x => x.Flags == 1024);
                if (tongkl != null)
                {
                    double tongklcl = 0;
                    double tongklxi = 0;
                    double tongklpg = 0;
                    double tongklnuoc = 0;
                    for (int i = 0; i < uniqueMa.Count; i++)
                    {
                        string paramname = $"TongKL.{uniqueMa[i].PhanLoai}";
                        report.SetParameterValue($"{paramname}.KL.{uniqueMa[i].SiloNo}", tongkl.GetTP(i));
                        report.SetParameterValue($"{paramname}.Ten.{uniqueMa[i].SiloNo}", uniqueMa[i].Ten);
                        switch (uniqueMa[i].PhanLoai)
                        {
                            case LoaiThanhPhan.CotLieu:
                                if (double.TryParse(tongkl.GetTP(i), out double klcl)) tongklcl += klcl;
                                break;
                            case LoaiThanhPhan.XiMang:
                                if (double.TryParse(tongkl.GetTP(i), out double klxi)) tongklxi += klxi;
                                break;
                            case LoaiThanhPhan.PhuGia:
                                if (double.TryParse(tongkl.GetTP(i), out double klpg)) tongklpg += klpg;
                                break;
                            case LoaiThanhPhan.Nuoc:
                                if (double.TryParse(tongkl.GetTP(i), out double klnuoc)) tongklnuoc += klnuoc;
                                break;
                        }
                    }
                    report.SetParameterValue("Tong.M3", tongkl.M3Tron);
                    report.SetParameterValue("Tong.Phieu", tongkl.PhieuMa);
                    report.SetParameterValue("Tong.CotLieu", Math.Round(tongklcl));
                    report.SetParameterValue("Tong.XiMang", Math.Round(tongklxi));
                    report.SetParameterValue("Tong.PhuGia", Math.Round(tongklpg, 1));
                    report.SetParameterValue("Tong.Nuoc", Math.Round(tongklnuoc));
                }

                // Gửi dữ liệu mẻ
                DataSet ds = new DataSet("dataset");

                var dt = FastReportPrintUtils.ConvertToDataTable<TKMeInDO>(dsme);
                dt.TableName = "TKMeInDO";
                ds.Tables.Add(dt);

                report.RegisterData(ds, "dataset", true);
                
                var dataSource = report.GetDataSource("TKMeInDO");
                var dataBand = report.FindObject("Data1") as DataBand;
                if (dataBand != null) dataBand.DataSource = dataSource;

                await report.PrepareAsync();
                using var ms = FastReportPrintUtils.GetReportAsPdfStreamAsync(report);
                PrintUtils.PrintPdfMemoryStream(ms, showDialog, 1);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "In báo cáo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
