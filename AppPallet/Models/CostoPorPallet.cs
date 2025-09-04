using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class CostoPorPallet
{
    public int CostoPorPalletId { get; set; }

    public string NombrePalletCliente { get; set; } = null!;

    public int CantidadPorDia { get; set; }

    public int CargaCamion { get; set; }

    public int MesId { get; set; }

    public int PalletId { get; set; }

    public int EmpresaId { get; set; }

    public decimal? PrecioPallet { get; set; }

    public int? GananciaPorCantPallet { get; set; }

    public int? CostoPorCamionId { get; set; }

    public virtual CostoPorCamion? CostoPorCamion { get; set; }

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual Me Mes { get; set; } = null!;

    public virtual Pallet Pallet { get; set; } = null!;
}
