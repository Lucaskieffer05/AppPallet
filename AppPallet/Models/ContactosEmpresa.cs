namespace AppPallet.Models;

public partial class ContactosEmpresa
{
    public int ContactosEmpresaId { get; set; }

    public int EmpresaId { get; set; }

    public int AreaId { get; set; }

    public string? Contacto { get; set; }

    public string? Mail { get; set; }

    public string? Telefono { get; set; }

    public string? Comentario { get; set; }

    public string? Pallet { get; set; }

    public string? Sello { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual Empresa Empresa { get; set; } = null!;
}
