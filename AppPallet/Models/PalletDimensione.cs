namespace AppPallet.Models;

public partial class PalletDimensione
{
    public int DimensionId { get; set; }

    public int? PalletId { get; set; }

    public string TipoDimension { get; set; } = null!;

    public decimal Valor { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public string Tolerancia { get; set; } = null!;

    public virtual Pallet? Pallet { get; set; }
}
