using System.Globalization;

namespace AppPallet.Converters
{
    public class BalanceColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                return balance >= 0 ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");
            }
            return Color.FromArgb("#666666");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}