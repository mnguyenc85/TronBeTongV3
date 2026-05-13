using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TronBeTongV3.Reports
{
    public static class ExcelExporter
    {
        public static void ExportDataGridVisibleText(DataGrid dataGrid, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Export"
                };
                sheets.Append(sheet);

                // ====== Ghi Header ======
                Row headerRow = new Row();
                foreach (var column in dataGrid.Columns)
                {
                    string header = column.Header?.ToString() ?? "";
                    headerRow.Append(CreateTextCell(header));
                }
                sheetData.Append(headerRow);

                // ====== Ghi dữ liệu từ cell hiển thị ======
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);

                    // Nếu virtualization đang bật thì có thể row chưa được tạo -> force
                    if (rowContainer == null)
                    {
                        dataGrid.UpdateLayout();
                        dataGrid.ScrollIntoView(dataGrid.Items[i]);
                        rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                    }

                    if (rowContainer == null) continue;

                    Row newRow = new Row();

                    foreach (var column in dataGrid.Columns)
                    {
                        FrameworkElement cellContent = column.GetCellContent(rowContainer);
                        string? cellText = "";

                        if (cellContent is TextBlock tb)
                            cellText = tb.Text;
                        else if (cellContent is CheckBox cb)
                            cellText = (cb.IsChecked == true) ? "True" : "False";
                        else if (cellContent != null)
                            cellText = cellContent.ToString();

                        cellText ??= "";
                        newRow.Append(CreateTextCell(cellText));
                    }

                    sheetData.Append(newRow);
                }

                workbookPart.Workbook.Save();
            }
        }

        private static Cell CreateTextCell(string text)
        {
            return new Cell()
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
        }
    }
}
