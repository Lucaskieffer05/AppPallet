using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class ClienteTopDTO
    {
        public int EmpresaId { get; set; }
        public string NomEmpresa { get; set; }
        public int TotalVentas { get; set; }
        public int TotalPallets { get; set; }
    }
}
