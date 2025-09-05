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

    public virtual ICollection<CostoPorPallet> CostoPorPallets { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<PalletComponente> PalletComponentes { get; set; } = new List<PalletComponente>();

    public virtual ICollection<PalletDimensione> PalletDimensiones { get; set; } = new List<PalletDimensione>();

    public virtual ICollection<PalletEspecificacione> PalletEspecificaciones { get; set; } = new List<PalletEspecificacione>();

    public virtual ICollection<PalletHumedad> PalletHumedads { get; set; } = new List<PalletHumedad>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
