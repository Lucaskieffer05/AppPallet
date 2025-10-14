using System.Globalization;

namespace AppPallet.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSuccess && isSuccess)
            {
                return Color.FromArgb("#10B981"); // Verde éxito
            }
            return Color.FromArgb("#EF4444"); // Rojo error
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}