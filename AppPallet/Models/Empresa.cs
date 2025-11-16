using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Empresa
{
    public int EmpresaId { get; set; }

    public string NomEmpresa { get; set; } = null!;

    public string? Cuit { get; set; }

    public string? Domicilio { get; set; }

    public DateTime? FechaDelete { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<ContactosEmpresa> ContactosEmpresa { get; set; } = new List<ContactosEmpresa>();

    public virtual ICollection<CostoPorPallet> CostoPorPallet { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<Lote> Lote { get; set; } = new List<Lote>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
