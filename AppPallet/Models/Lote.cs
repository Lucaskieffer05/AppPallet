namespace AppPallet.Models;

public partial class Lote
{
    public int LoteId { get; set; }

    public int NumLote { get; set; }

    public DateOnly FechaSolicitada { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public string NomProveedor { get; set; } = null!;

    public decimal? PrecioProveedor { get; set; }

    public string? NumFacturaProveedor { get; set; }

    public string NomCamionero { get; set; } = null!;

    public decimal? PrecioCamionero { get; set; }

    public int PedidoId { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;
}
