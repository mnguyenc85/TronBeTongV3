using System.IO;
using System.Windows.Controls;

namespace NMPrinting
{
    public class PrintUtils
    {
        public static void PrintPdfMemoryStream(MemoryStream ms, bool showDialog = false, int solan = 1)
        {
            var doc = PdfiumViewer.Core.PdfDocument.Load(ms);
            using (var printDocument = doc.CreatePrintDocument())
            {
                if (showDialog)
                {
                    var printDialog = new PrintDialog();

                    if (printDialog.ShowDialog() == true)
                    {
                        for (int i = 0; i < solan; i++)
                            printDocument.Print();
                    }
                }
                else
                {
                    for (int i = 0; i < solan; i++)
                        printDocument.Print();
                }
            }
        }
    }
}
