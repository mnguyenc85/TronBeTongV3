using System;
using System.IO;
using System.Windows;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for ReportPreviewWnd.xaml
    /// </summary>
    public partial class WndReportPreview : Window
    {
        // Flag to track programmatic close
        private bool _isProgrammaticClose = false;

        public WndReportPreview()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void SetPdf(MemoryStream ms)
        {            
            PdfViewer.ShowPdf(ms);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PdfViewer.CleanMemory();
            if (!_isProgrammaticClose)  // If user clicks the close button
            {
                e.Cancel = true;        // Cancel closing
                Hide();                 // Hide the window instead
            }
        }

        public void CloseWindowProgrammatically()
        {
            _isProgrammaticClose = true;        // Set flag to allow closing
            Close();                            // Now the window will close normally
        }
    }
}
