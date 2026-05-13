using System;
using System.Windows.Media;

namespace TronBeTongV3.Core
{
    public class MyBrushes
    {
        public static SolidColorBrush BrTransparentOff { get; private set; } = new(Color.FromArgb(127, 127, 127, 127));
        public static SolidColorBrush BrTransparentOn { get; private set; } = new(Color.FromArgb(127, 0, 255, 0));

        public static SolidColorBrush BrOff { get; private set; } = new(Color.FromArgb(255, 224, 224, 224));
        public static SolidColorBrush BrOn { get; private set; } = new(Color.FromArgb(255, 50, 225, 50));
    }
}
