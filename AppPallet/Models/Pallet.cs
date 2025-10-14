using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Pallet
{
    public int PalletId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Estructura { get; set; }

    public string? Tratamiento { get; set; }

    public string? Sello { get; set; }

    public decimal? Peso { get; set; }

    public decimal? ToleranciaPeso { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int Stock { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public virtual ICollection<CostoPorPallet> CostoPorPallet { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<PalletComponentes> PalletComponentes { get; set; } = new List<PalletComponentes>();

    public virtual ICollection<PalletDimensiones> PalletDimensiones { get; set; } = new List<PalletDimensiones>();

    public virtual ICollection<PalletEspecificaciones> PalletEspecificaciones { get; set; } = new List<PalletEspecificaciones>();

    public virtual ICollection<PalletHumedad> PalletHumedad { get; set; } = new List<PalletHumedad>();

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
