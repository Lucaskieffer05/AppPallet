using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AppPallet.Models;

public partial class CostoPorPallet
{
    public int CostoPorPalletId { get; set; }

    public string NombrePalletCliente { get; set; } = null!;

    public int CantidadPorDia { get; set; }

    public int CargaCamion { get; set; }

    public int PalletId { get; set; }

    public int EmpresaId { get; set; }

    public decimal? PrecioPallet { get; set; }

    public decimal? GananciaPorCantPallet { get; set; }

    public DateTime Mes { get; set; }

    public int HorasPorMes { get; set; }

    public virtual ObservableCollection<CostoPorCamion> CostoPorCamions { get; set; } = new ObservableCollection<CostoPorCamion>();

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual Pallet Pallet { get; set; } = null!;


}
