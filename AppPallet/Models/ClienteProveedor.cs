using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class ClienteProveedor
{
    public int ClienteProveedorId { get; set; }

    public string NomEmpresa { get; set; } = null!;

    public string? Cuit { get; set; }

    public string? Domicilio { get; set; }

    public string Area { get; set; } = null!;

    public string? Nombres { get; set; }

    public string? Mail { get; set; }

    public string? Telefono { get; set; }

    public string? Pallets { get; set; }

    public string? Sello { get; set; }

    public string? Comentario { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<CostoPorPallet> CostoPorPallets { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
