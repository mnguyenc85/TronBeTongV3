using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;

namespace DebuggerViewer.ViewModel
{
    public class NMPlotController
    {
        public PlotModel Model { get; private set; }
        public Axis AxisX { get; private set; }
        public Axis AxisY0 { get; private set; }
        public Axis AxisY1 { get; private set; }

        public NMPlotController()
        {
            Model = new PlotModel();

            Model.Legends.Add(new Legend()
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
            });

            AxisX = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "T (s)",
                TitleColor = OxyColors.Blue,
                MajorGridlineStyle = LineStyle.Solid,
                //MinorGridlineStyle = LineStyle.Dot,
                Minimum = 0,
                Maximum = 60
            };
            AxisY0 = new LinearAxis()
            {
                Key = "Y0",
                Position = AxisPosition.Left,
                Title = "Int16",
                TitleColor = OxyColors.Blue,
                MajorGridlineStyle = LineStyle.Solid,
                //MinorGridlineStyle = LineStyle.Dot,
                AxislineStyle = LineStyle.Solid,
                Minimum = -0.5,
                Maximum = 16
            };
            AxisY1 = new LinearAxis()
            {
                Key = "Y1",
                Position = AxisPosition.Right,
                PositionTier = 0,
                Title = "Real",
                TitleColor = OxyColors.Red,
                //MajorGridlineStyle = LineStyle.Solid,
                //MinorGridlineStyle = LineStyle.Dot,
                AxislineStyle = LineStyle.Solid,
                Minimum = -20,
                Maximum = 2000
            };
            Model.Axes.Add(AxisX);
            Model.Axes.Add(AxisY0);
            Model.Axes.Add(AxisY1);

        }

        public void Invalidate()
        {
            Model.InvalidatePlot(true);
        }
    }
}
