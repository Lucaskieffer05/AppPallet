using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Cheque
{
    public int ChequeId { get; set; }

    public string Proveedor { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string NumCheque { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime FechaEmision { get; set; }

    public DateTime FechaPago { get; set; }

    public int? Estado { get; set; }
}
