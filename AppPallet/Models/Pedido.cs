using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int EmpresaId { get; set; }

    public int PalletId { get; set; }

    public DateTime FechaEntrega { get; set; }

    public int Cantidad { get; set; }

    public virtual Empresa Empresa { get; set; } = null!;

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    public virtual Pallet Pallet { get; set; } = null!;
}
