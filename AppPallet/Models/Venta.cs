using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Venta
{
    public int VentaId { get; set; }

    public DateTime FechaVenta { get; set; }

    public int CantPallets { get; set; }

    public string Estado { get; set; } = null!;

    public int? CostoPorPalletId { get; set; }

    public decimal? PrecioManual { get; set; }

    public int? EmpresaId { get; set; }

    public string? Comentario { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }

    public DateTime? FechaCobroEstimada { get; set; }

    public string? NumeroOrden { get; set; }

    public string? NumeroRemito { get; set; }

    public string? NumeroFactura { get; set; }

    public virtual CostoPorPallet? CostoPorPallet { get; set; }

    public virtual Empresa? Empresa { get; set; }

    public decimal PrecioUnitarioMostrar => (decimal)(CostoPorPallet?.PrecioPallet ?? PrecioManual ?? 0);

    public string EmpresaNombreMostrar => CostoPorPallet?.Empresa?.NomEmpresa ?? Empresa?.NomEmpresa ?? string.Empty;
}
