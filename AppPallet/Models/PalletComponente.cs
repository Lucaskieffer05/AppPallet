namespace AppPallet.Models;

public partial class PalletComponente
{
    public int ComponenteId { get; set; }

    public int? PalletId { get; set; }

    public string TipoComponente { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal Espesor { get; set; }

    public decimal Ancho { get; set; }

    public decimal Largo { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public virtual Pallet? Pallet { get; set; }
}
