namespace AppPallet.Models;

public partial class CostoPorCamion
{
    public int CostoPorCamionId { get; set; }

    public string NombreCosto { get; set; } = null!;

    public decimal Monto { get; set; }

    public virtual ICollection<CostoPorPallet> CostoPorPallets { get; set; } = new List<CostoPorPallet>();
}
