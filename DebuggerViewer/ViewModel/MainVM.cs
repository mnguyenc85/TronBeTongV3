using DebuggerViewer.Data;
using NMWPFControls.Core.MVVM;
using OxyPlot;
using OxyPlot.Series;
using System.Globalization;
using System.Windows;
using System.Windows.Shapes;

namespace DebuggerViewer.ViewModel
{
    public class MainVM: VMBase
    {
        private NMPlotController _plot = new();
        public Dictionary<string, DataLine> Lines { get; private set; } = [];

        public MainVM() {
        }

        public void Init(OxyPlot.Wpf.PlotView plot)
        {
            plot.Model = _plot.Model;
        }

        public void ShowPlot()
        {
            _plot.Model.Series.Clear();

            foreach (var (k, l) in Lines)
            {
                var series = new LineSeries
                {
                    Title = k,
                    YAxisKey = l.Type == 0? "Y0": "Y1",
                };

                foreach (var pt in l.DataPoints)
                {
                    series.Points.Add(new OxyPlot.DataPoint(pt.X, pt.Y));
                }

                _plot.Model.Series.Add(series);
            }

            _plot.Invalidate();
        }

        public void LoadDataFromClipBoard()
        {
            Lines.Clear();
            string s = Clipboard.GetText();
            LoadDataFromString(s);
        }
        public void LoadDataFromFile(string filename)
        {

        }

        private void LoadDataFromString(string s)
        {
            string[] lines = s.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

            DataLine? curdLine = null;
            string? curName = null;

            foreach (string l in lines)
            {
                if (string.IsNullOrWhiteSpace(l))
                    continue;

                // Dòng bắt đầu bằng '#' => tên line mới
                if (l.StartsWith("#"))
                {
                    curName = l.Substring(1).Trim();
                    curdLine = new DataLine();
                    Lines[curName] = curdLine;
                }
                else if (curdLine != null)
                {
                    // Đọc giá trị t, v
                    var parts = l.Split(',');

                    if (parts.Length >= 2 &&
                        double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double t) &&
                        double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                    {
                        if (v > 20) curdLine.Type = 1;
                        curdLine.DataPoints.Add(new Vector2d(t, v));
                    }
                }
            }
        }
    }
}
