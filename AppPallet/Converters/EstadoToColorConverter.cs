using System.Globalization;

namespace AppPallet.Converters
{
    public class EstadoToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int estado)
            {
                return estado switch
                {
                    0 => Color.FromArgb("#E8F5E9"), // Verde claro
                    1 => Color.FromArgb("#FFE553"), // Rojo claro
                    2 => Color.FromArgb("#FFA353"), // Gris
                    3 => Color.FromArgb("#D65265"), // Azul claro
                    4 => Color.FromArgb("#58D651"), // Azul claro
                    _ => Colors.Transparent
                };
            }
            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}