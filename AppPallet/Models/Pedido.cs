using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int PalletId { get; set; }

    public DateTime? FechaEInicio { get; set; }

    public int Cantidad { get; set; }

    public int LoteId { get; set; }

    public DateTime? FechaEFinal { get; set; }

    public virtual ICollection<HistorialHumedad> HistorialHumedad { get; set; } = new List<HistorialHumedad>();

    public virtual Lote Lote { get; set; } = null!;

    public virtual Pallet Pallet { get; set; } = null!;
}
