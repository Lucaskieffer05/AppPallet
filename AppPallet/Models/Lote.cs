using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Lote
{
    public int LoteId { get; set; }

    public int NumLote { get; set; }

    public DateTime FechaSolicitada { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string NomProveedor { get; set; } = null!;

    public decimal? PrecioProveedor { get; set; }

    public string? NumFacturaProveedor { get; set; }

    public string NomCamionero { get; set; } = null!;

    public decimal? PrecioCamionero { get; set; }

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
