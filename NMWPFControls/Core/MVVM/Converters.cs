using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace NMWPFControls.Core.MVVM
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToNotVisConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntToVisConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            int p = int.Parse((string)parameter);
            return v == p ? Visibility.Visible: Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}
