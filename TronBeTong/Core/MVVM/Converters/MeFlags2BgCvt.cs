using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TronBeTongV3.Core.MVVM.Converters
{
    [ValueConversion(typeof(int), typeof(Brush))]
    public class MeFlags2BgCvt: MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int flags)
            {
                if ((flags & 4) == 4)
                {
                    if ((flags & 2) == 2) return Brushes.LightGreen;
                    return Brushes.SkyBlue;
                }
                else if ((flags & 2) == 2) return Brushes.Yellow;
                else if ((flags & 64) == 64) return Brushes.LightGray;           // Tên TP
                else if ((flags & 128) == 128) return Brushes.LightGray;         // Tổng
                else if ((flags & 256) == 256) return Brushes.WhiteSmoke;          // CP chuẩn
                else if ((flags & 512) == 512) return Brushes.WhiteSmoke;          // CP mẻ
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
