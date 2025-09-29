using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class GastosFijos
{
    public int GastosFijosId { get; set; }

    public string NombreGastoFijo { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime Mes { get; set; }
}
