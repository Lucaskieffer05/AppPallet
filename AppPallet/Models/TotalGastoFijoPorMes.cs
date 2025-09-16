using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class TotalGastoFijoPorMes
    {
        public TotalGastoFijoPorMes()
        {
        }
        public decimal TotalGastoFijo { get; set; }
        public DateTime Mes { get; set; }
    }
}
