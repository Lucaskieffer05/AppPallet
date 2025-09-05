using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class CostoPorCamion
{
    public int CostoPorCamionId { get; set; }

    public string NombreCosto { get; set; } = null!;

    public decimal Monto { get; set; }

    public int CostoPorPalletId { get; set; }

    public virtual CostoPorPallet CostoPorPallet { get; set; } = null!;
}
