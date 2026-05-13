using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TronBeTongV3.Core.MVVM.Converters
{
    [ValueConversion(typeof(LoaiThanhPhan), typeof(string))]
    public class AggrTypeToStringCvt : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (LoaiThanhPhan)value;
            switch (v)
            {
                case LoaiThanhPhan.CotLieu: return "Cốt liệu";
                case LoaiThanhPhan.XiMang: return "Xi măng";
                case LoaiThanhPhan.PhuGia: return "Phụ gia";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as string;
            switch (v)
            {
                case "Cốt liệu": return LoaiThanhPhan.CotLieu;
                case "Xi măng": return LoaiThanhPhan.XiMang;
                case "Phụ gia": return LoaiThanhPhan.PhuGia;
            }
            return LoaiThanhPhan.None;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
