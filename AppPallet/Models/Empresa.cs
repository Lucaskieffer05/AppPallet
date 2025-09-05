namespace AppPallet.Models;

public partial class Empresa
{
    public int EmpresaId { get; set; }

    public string NomEmpresa { get; set; } = null!;

    public string? Cuit { get; set; }

    public string? Domicilio { get; set; }

    public virtual ICollection<ContactosEmpresa> ContactosEmpresas { get; set; } = new List<ContactosEmpresa>();

    public virtual ICollection<CostoPorPallet> CostoPorPallets { get; set; } = new List<CostoPorPallet>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
