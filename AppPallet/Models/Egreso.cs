using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Egreso
{
    public string DescripEgreso { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public string? Factura { get; set; }

    public decimal Monto { get; set; }

    public decimal? SumaIva { get; set; }

    public DateTime? Mes { get; set; }

    public string? Comentario { get; set; }
}
