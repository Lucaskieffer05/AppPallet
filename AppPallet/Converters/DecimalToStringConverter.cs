using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AppPallet.Converters
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return dec.ToString("#,##0.##", culture);
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && decimal.TryParse(str, NumberStyles.Any, culture, out var dec))
                return dec;
            return 0m;
        }
    }
}