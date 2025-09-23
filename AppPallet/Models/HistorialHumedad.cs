using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class HistorialHumedad
{
    public int HistorialHumedadId { get; set; }

    public DateTime Fecha { get; set; }

    public int Min { get; set; }

    public int Max { get; set; }

    public decimal? Promedio { get; set; }

    public string? PesoAprox { get; set; }

    public int PedidoId { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;
}
