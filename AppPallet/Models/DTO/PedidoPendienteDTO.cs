using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class PedidoPendienteDTO
    {
        public string? NombrePallet { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaSolicitada { get; set; }
        public int NumLote { get; set; }
        public int DiasConcurridos => (DateTime.Now - FechaSolicitada).Days;

    }
}
