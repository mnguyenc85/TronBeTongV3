using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.Reports
{
    public static class CsvExporter
    {
        #region Khách hàng
        public static void Export_KhachHang(string filePath, List<KDKhachHangVM> customers)
        {
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            // Ghi header
            writer.WriteLine("STT,Ma,Ten,DiaChi,Sdt,Email,MaSoThue,LienHe,GhiChu");

            // Ghi dữ liệu
            foreach (var c in customers)
            {
                string line = string.Join(",",
                    Escape(c.STT.ToString()),
                    Escape(c.Ma),
                    Escape(c.Ten),
                    Escape(c.DiaChi),
                    Escape(c.Sdt),
                    Escape(c.Email),
                    Escape(c.MaSoThue),
                    Escape(c.LienHe),
                    Escape(c.GhiChu)
                );
                writer.WriteLine(line);
            }
        }

        public static List<KDKhachHangVM> Import_KhachHang(string filePath)
        {
            var customers = new List<KDKhachHangVM>();

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            string? headerLine = reader.ReadLine(); // bỏ dòng header
            if (headerLine == null) return customers;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = ParseCsvLine(line);
                if (fields.Count < 9) continue; // tránh lỗi nếu thiếu cột

                var c = new KDKhachHangVM
                {
                    STT = int.TryParse(fields[0], out var stt) ? stt : 0,
                    Ma = fields[1],
                    Ten = fields[2],
                    DiaChi = fields[3],
                    Sdt = fields[4],
                    Email = fields[5],
                    MaSoThue = fields[6],
                    LienHe = fields[7],
                    GhiChu = fields[8]
                };

                customers.Add(c);
            }

            return customers;
        }
        #endregion

        #region Dự án
        public static void Export_DuAn(string filePath, List<KDDuAnVM> projects)
        {
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));

            // Ghi header
            writer.WriteLine("STT,DuAn,CongTrinh,HangMuc,DiaChi,GhiChu,MaKH");

            // Ghi dữ liệu
            foreach (var c in projects)
            {
                string line = string.Join(",",
                    Escape(c.STT.ToString()),
                    Escape(c.DuAn),
                    Escape(c.CongTrinh),
                    Escape(c.HangMuc),
                    Escape(c.DiaChi),
                    Escape(c.GhiChu),
                    Escape(c.KHMa)
                );
                writer.WriteLine(line);
            }
        }
        
        public static List<KDDuAnVM> Import_DuAn(string filePath)
        {
            var projects = new List<KDDuAnVM>();

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            string? headerLine = reader.ReadLine(); // bỏ dòng header
            if (headerLine == null) return projects;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = ParseCsvLine(line);
                if (fields.Count < 7) continue; // tránh lỗi nếu thiếu cột

                var c = new KDDuAnVM
                {
                    STT = int.TryParse(fields[0], out var stt) ? stt : 0,
                    DuAn = fields[1],
                    CongTrinh = fields[2],
                    HangMuc = fields[3],
                    DiaChi = fields[4],
                    GhiChu = fields[5],
                    KHMa = fields[6],
                };

                projects.Add(c);
            }

            return projects;
        }        
        #endregion

        private static string Escape(string? s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Replace("\"", "\"\""); // escape dấu "
            if (s.Contains(",") || s.Contains("\n") || s.Contains("\r"))
                return $"\"{s}\"";
            return s;
        }

        // Hàm parse CSV line (xử lý dấu nháy kép, dấu phẩy trong chuỗi)
        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"'); // xử lý escape ""
                        i++;
                    }
                    else if (c == '"')
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            result.Add(sb.ToString());
            return result;
        }
    }
}
