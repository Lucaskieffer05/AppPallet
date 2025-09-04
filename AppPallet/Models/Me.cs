using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Me
{
    public int MesId { get; set; }

    public DateOnly FechaMes { get; set; }

    public int? HorasPorMes { get; set; }

    public virtual ICollection<CostoPorPallet> CostoPorPallets { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<GastosFijo> GastosFijos { get; set; } = new List<GastosFijo>();
}
