using System;

namespace DebuggerViewer.Data
{
    public struct Vector2d(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
    }
}
