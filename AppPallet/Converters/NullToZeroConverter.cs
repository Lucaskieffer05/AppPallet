using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AppPallet.Converters
{
    public class NullToZeroConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
}