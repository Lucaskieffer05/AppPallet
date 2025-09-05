namespace AppPallet.Models;

public partial class PalletHumedad
{
    public int HumedadId { get; set; }

    public int? PalletId { get; set; }

    public string TipoComponente { get; set; } = null!;

    public decimal HumedadMaxima { get; set; }

    public string Tolerancia { get; set; } = null!;

    public virtual Pallet? Pallet { get; set; }
}
