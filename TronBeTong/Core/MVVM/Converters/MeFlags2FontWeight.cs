using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TronBeTongV3.Core.MVVM.Converters
{
    [ValueConversion(typeof(int), typeof(FontWeights))]
    public class MeFlags2FontWeight: MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int flags)
            {
                if ((flags & 128) == 128) return FontWeights.DemiBold;         // Tổng
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
