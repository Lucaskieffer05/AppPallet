using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class PedidoMostrarDTO : Pedido
    {
        public string NombrePallet { get; set; } = string.Empty;
        public string Estado
        {
            get
            {
                if (!FechaEInicio.HasValue)
                    return "Sin Empezar";
                if (FechaEFinal.HasValue)
                    return "Completado";
                return "En Proceso";
            }
        }

        public string EstadoColor
        {
            get
            {
                if (!FechaEInicio.HasValue)
                    return "#9E9E9E"; // Gris para "Sin Empezar"
                if (FechaEFinal.HasValue)
                    return "#4CAF50"; // Verde para "Completado"
                return "#FF9800";    // Naranja para "En Proceso"
            }
        }

        public PedidoMostrarDTO() { }

        public PedidoMostrarDTO(Pedido pedido)
        {
            PedidoId = pedido.PedidoId;
            PalletId = pedido.PalletId;
            FechaEInicio = pedido.FechaEInicio;
            Cantidad = pedido.Cantidad;
            LoteId = pedido.LoteId;
            FechaEFinal = pedido.FechaEFinal;
            Pallet = pedido.Pallet;
            NombrePallet = pedido.Pallet?.Nombre ?? "N/A";
        }
    }
}
