using System.Globalization;

namespace AppPallet.Converters
{
    public class NullableDateTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is DateTime date ? date : DateTime.MinValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime date && date != DateTime.MinValue)
                return date;
            return null;
        }
    }
}
