using System.Data;
using System.IO;
using FastReport;

namespace TronBeTongV3.Core
{
    public class FastReportPrintUtils
    {
        /// <summary>
        /// Xuất FastReport ra file .pdf trong bộ nhớ
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static MemoryStream GetReportAsPdfStreamAsync(Report report)
        {
            FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new();

            MemoryStream ms = new();

            // Configure export if needed
            pdfExport.Export(report, ms);
            ms.Position = 0; // Reset stream position

            return ms;
        }


        /// <summary>
        /// Chuyển List thành DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(List<T> list)
        {
            var dt = new DataTable(typeof(T).Name);
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in list)
            {
                var row = dt.NewRow();
                foreach (var prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
