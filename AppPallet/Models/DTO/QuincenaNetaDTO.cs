using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class QuincenaNetoDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Numero { get; set; }
        public decimal TotalActivos { get; set; }
        public decimal TotalPasivos { get; set; }
        public decimal TotalNeto { get; set; }

        public string Titulo
        {
            get
            {
                var mes = new DateTime(Año, Mes, 1).ToString("MMMM");
                var mesCapitalizado = char.ToUpper(mes[0]) + mes.Substring(1);
                return $"{mesCapitalizado} - {(Numero == 1 ? "1ra" : "2da")} quincena";
            }
        }

        public string DisplayTotalNeto => TotalNeto >= 0
            ? $"Neto: ${TotalNeto:N2}"
            : $"Pérdida: ${Math.Abs(TotalNeto):N2}";
    }
}
