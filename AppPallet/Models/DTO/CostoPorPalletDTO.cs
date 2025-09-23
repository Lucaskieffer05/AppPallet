using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class CostoPorPalletDTO
{
    public int CostoPorPalletId { get; set; }

    public string NombrePalletCliente { get; set; } = null!;

    public string? FechaNombre { get; set; } 

    public int CantidadPorDia { get; set; }

    public int CargaCamion { get; set; }

    public int PalletId { get; set; }

    public int EmpresaId { get; set; }

    public decimal? PrecioPallet { get; set; }

    public decimal? GananciaPorCantPallet { get; set; }

    public DateTime Mes { get; set; }

    public int HorasPorMes { get; set; }

    public virtual ICollection<CostoPorCamion> CostoPorCamions { get; set; } = new List<CostoPorCamion>();

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual Pallet Pallet { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
