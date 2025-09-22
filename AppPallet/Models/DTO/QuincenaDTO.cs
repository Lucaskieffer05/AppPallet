using System.Collections.ObjectModel;

namespace AppPallet.Models
{
    public class QuincenaDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Numero { get; set; } // 1ra o 2da quincena

        public ObservableCollection<ActivoPasivo> Items { get; set; } = new();

        public decimal Total => Items.Sum(x => x.Monto);

        // Nueva propiedad para el total neto (se calculará externamente)
        public decimal TotalNeto { get; set; }

        public string Titulo
        {
            get
            {
                var mes = new DateTime(Año, Mes, 1).ToString("MMMM");
                var mesCapitalizado = char.ToUpper(mes[0]) + mes.Substring(1);
                return $"{mesCapitalizado} - {(Numero == 1 ? "1ra" : "2da")} quincena";
            }
        }
    }
}