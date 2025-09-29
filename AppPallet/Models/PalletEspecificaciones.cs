using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class PalletEspecificaciones
{
    public int EspecificacionId { get; set; }

    public int? PalletId { get; set; }

    public string? TipoClavo { get; set; }

    public string? OtrasEspecificaciones { get; set; }

    public virtual Pallet? Pallet { get; set; }
}
