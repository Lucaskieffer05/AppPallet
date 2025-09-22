using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class TotalGastoFijoPorMesDTO
    {
        public TotalGastoFijoPorMesDTO()
        {
        }
        public decimal TotalGastoFijo { get; set; } = 0;
        public DateTime Mes { get; set; }
    }
}
