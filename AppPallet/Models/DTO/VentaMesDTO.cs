using AppPallet.Models;

namespace AppPallet.Models
{
    public class VentaMesDTO
    {
        public int Mes { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalPallets { get; set; }
        public List<Venta>? Ventas { get; set; }
    }
}