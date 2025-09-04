using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Area
{
    public int AreaId { get; set; }

    public string NomArea { get; set; } = null!;

    public virtual ICollection<ContactosEmpresa> ContactosEmpresas { get; set; } = new List<ContactosEmpresa>();
}
