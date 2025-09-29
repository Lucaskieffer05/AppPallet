using System.Globalization;


namespace AppPallet.Converters
{
    public class EstadoColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime fechaEntrega)
            {
                return "#4CAF50"; // Verde si entregado
            }
            else if (value == null)
            {
                return "#FF9800"; // Naranja si pendiente
            }
            return "#FF9800";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EstadoTextoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime fechaEntrega)
            {
                return "Entregado";
            }
            else if (value == null)
            {
                return "Pendiente";
            }
            return "Pendiente";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DuracionEstimadaConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime fechaInicio)
            {
                var duracion = DateTime.Today - fechaInicio;
                if (duracion.TotalDays < 0)
                    return "Programado para el futuro";
                else if (duracion.TotalDays == 0)
                    return "Comienza hoy";
                else
                    return $"En proceso: {duracion.Days} días";
            }
            return "Fecha no especificada";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool invert = parameter != null && bool.TryParse(parameter.ToString(), out bool result) && result;

            if (value == null)
                return invert;

            return !invert;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;

            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;

            return false;
        }
    }
}
