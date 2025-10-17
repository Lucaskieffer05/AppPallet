using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class LoteMostrarDTO : Lote
    {
        public int TotalPedidos { get; set; }
        public string NomProveedor { get; set; } = null!;
        public int TotalPallets { get; set; }
        public decimal? CostoTotal => (PrecioProveedor ?? 0) + (PrecioCamionero ?? 0);
        public string Estado => FechaEntrega.HasValue ? "Entregado" : "Pendiente";
        public string EstadoColor => FechaEntrega.HasValue ? "#4CAF50" : "#FF9800";
        public TimeSpan? TiempoProcesamiento => FechaEntrega.HasValue ?
            FechaEntrega.Value - FechaSolicitada : DateTime.Now - FechaSolicitada;
        public bool TieneFechaEntrega => FechaEntrega.HasValue;
        public List<Pedido> PedidosDetallados { get; set; } = [];
    }
}
