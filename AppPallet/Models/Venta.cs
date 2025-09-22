using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Venta
{
    public int VentaId { get; set; }

    public DateTime FechaVenta { get; set; }

    public int CantPallets { get; set; }

    public string Estado { get; set; } = null!;

    public int CostoPorPalletId { get; set; }

    public string? Comentario { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public virtual CostoPorPallet CostoPorPallet { get; set; } = null!;
}
