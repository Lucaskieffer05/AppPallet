using AppPallet.ViewModels;
using System.Globalization;

namespace AppPallet.Converters
{
    public class PendientesConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Se espera que el parámetro sea el LoteViewModel
            if (parameter is LoteViewModel vm)
            {
                return vm.ListaLotes.Count(l => !l.FechaEntrega.HasValue).ToString();
            }
            return "0";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}