using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Ingreso
{
    public string DescripIngreso { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public string? Op { get; set; }

    public string? Remito { get; set; }

    public string? Factura { get; set; }

    public decimal Monto { get; set; }

    public string? Comentario { get; set; }
}
