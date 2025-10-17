using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class ActivoPasivoMensualDTO
    {
        public int Mes { get; set; }
        public decimal TotalActivo { get; set; }
        public decimal TotalPasivo { get; set; }
        public decimal TotalCapitalNeta { get; set; }
    }
}
