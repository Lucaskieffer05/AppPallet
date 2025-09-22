using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class ActivoPasivo
{
    public int ActivoPasivoId { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime Mes { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Monto { get; set; }

    public string Categoria { get; set; } = null!;
}
