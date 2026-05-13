using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PdfiumViewer.Core;

namespace NMPrinting.Controls
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl, INotifyPropertyChanged
    {
        private int _p;
        public int CurPage { get { return _p; } set { if (_p != value) { _p = value; PageChanged(); } } }
        private List<double> _zooms = new List<double>() { 0.25, 0.5, 0.75, 1, 1.5, 2, 3, 4 };

        public PdfViewer()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
        }

        public void CleanMemory()
        {
            if (PdfRenderer.Document != null)
            {
                PdfRenderer.Document.Dispose();
                PdfRenderer.Document = null;
            }
        }

        public void ShowPdf(MemoryStream ms)
        {
            try
            {
                CleanMemory();

                var doc = PdfiumViewer.Core.PdfDocument.Load(ms);
                PdfRenderer.Document = doc;
                LblPageCount.Text = $" / {doc.PageCount}";
                TxtZoom.Text = (PdfRenderer.Zoom * 100).ToString();

                PdfRenderer.InvalidateArrange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtMoveFirst_Click(object sender, RoutedEventArgs e)
        {
            if (PdfRenderer.Page > 0) PdfRenderer.Page = 0;
        }

        private void BtMovePrev_Click(object sender, RoutedEventArgs e)
        {
            if (PdfRenderer.Page > 0) PdfRenderer.Page--;
        }

        private void BtMoveNext_Click(object sender, RoutedEventArgs e)
        {
            if (PdfRenderer.Page < PdfRenderer.PageCount - 1) PdfRenderer.Page++;
        }

        private void MoveEnd_Click(object sender, RoutedEventArgs e)
        {
            if (PdfRenderer.Page < PdfRenderer.PageCount - 1) PdfRenderer.Page = PdfRenderer.PageCount - 1;
        }

        private void BtPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintPdfWithDialog((PdfDocument)PdfRenderer.Document);
        }

        private void TxtCurPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TxtCurPageChanged();
        }

        private void TxtCurPage_LostFocus(object sender, RoutedEventArgs e)
        {
            TxtCurPageChanged();
        }

        private void TxtCurPageChanged() {
            if (int.TryParse(TxtCurPage.Text, out int p))
            {
                if (p >= 0 && p < PdfRenderer.PageCount)
                {
                    CurPage = p;
                }
            }
        }
        private void PageChanged() {
            TxtCurPage.Text = (_p + 1).ToString();
            NotifyChanged("CurPage");
        }

        public void PrintPdfWithDialog(PdfDocument doc)
        {
            using (var printDocument = doc.CreatePrintDocument())
            {
                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    printDocument.Print();
                }
            }
        }

        private void BtFitWidth_Click(object sender, RoutedEventArgs e)
        {
            FitWidth(CurPage);
        }

        private void BtFitHeight_Click(object sender, RoutedEventArgs e)
        {
            FitHeight(CurPage);
        }


        private void BtZoomIn_Click(object sender, RoutedEventArgs e)
        {
            var z = FindZoom(PdfRenderer.Zoom, true);
            Zoom(z);
        }

        private void BtZoomOut_Click(object sender, RoutedEventArgs e)
        {
            var z = FindZoom(PdfRenderer.Zoom, false);
            Zoom(z);
        }

        private void FitWidth(int page)
        {
            var size = PdfRenderer.Document.Pages[page].Size;

            if (size.Width > 0)
            {
                double z = Math.Round((PdfRenderer.ActualWidth - 12) / size.Width, 3);
                Zoom(z);
            }
        }
        private void FitHeight(int page)
        {
            var size = PdfRenderer.Document.Pages[page].Size;

            if (size.Height > 0)
            {
                double z = Math.Round((PdfRenderer.ActualHeight - 12) / size.Height, 3);
                Zoom(z);
            }
        }

        private double FindZoom(double cz, bool zoomin = true)
        {
            if (zoomin)
            {
                for (int i = 0; i < _zooms.Count - 1; i++)
                {
                    if (_zooms[i] > cz) return _zooms[i];
                }
                return _zooms[_zooms.Count - 1];
            }
            else
            {
                for (int i = 1; i < _zooms.Count; i++)
                {
                    if (_zooms[i] >= cz) return _zooms[i - 1];
                }
                return _zooms[0];
            }
        }

        public void Zoom(double z)
        {
            PdfRenderer.Zoom = z;
            TxtZoom.Text = (z * 100).ToString();
        }

        #region INotifyPropertyChanged
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = new Dictionary<string, PropertyChangedEventArgs>();

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
    }
}
