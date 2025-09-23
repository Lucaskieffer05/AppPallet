using System;
using System.Collections.Generic;

namespace AppPallet.Models;

public partial class Stock
{
    public int StockId { get; set; }

    public int Stock1 { get; set; }

    public virtual ICollection<Pallet> Pallets { get; set; } = new List<Pallet>();
}
