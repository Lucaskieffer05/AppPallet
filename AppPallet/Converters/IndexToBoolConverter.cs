using System.Globalization;

namespace AppPallet.Converters
{
    public class IndexToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && parameter is string parameterString)
            {
                if (int.TryParse(parameterString, out int targetIndex))
                {
                    return selectedIndex == targetIndex;
                }
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isChecked && (bool)isChecked && parameter is string parameterString)
            {
                if (int.TryParse(parameterString, out int targetIndex))
                {
                    return targetIndex;
                }
            }
            return Binding.DoNothing; // Mejor usar DoNothing en lugar de -1
        }
    }
}