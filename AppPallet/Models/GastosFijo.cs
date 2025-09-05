namespace AppPallet.Models;

public partial class GastosFijo
{
    public int GastosFijosId { get; set; }

    public string NombreGastoFijo { get; set; } = null!;

    public decimal Monto { get; set; }

    public int MesId { get; set; }

    public virtual Mes Mes { get; set; } = null!;
}
