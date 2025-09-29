using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class GastosYCostosDTO
    {
        public CostoPorPallet? Costo { get; set; }
        public TotalGastoFijoPorMesDTO? TotalGasto { get; set; }

        public int MesToCopy { get; set; } = DateTime.Now.Month;
        public int AñoToCopy { get; set; } = DateTime.Now.Year;
        public int AñoToCopyIndex { get; set; } = 1;

        public decimal CantDiasCamion
        {
            get
            {
                if (Costo == null || Costo.CantidadPorDia == 0)
                    return 0;
                return (decimal)Costo.CargaCamion / Costo.CantidadPorDia;
            }
        }

        public decimal HacenPorMes
        {
            get
            {
                if (Costo == null || Costo.CantidadPorDia == 0)
                    return 0;
                return ((decimal)Costo.CantidadPorDia / 9) * Costo.HorasPorMes; ;
            }
        }

        public decimal GastoDelPallet
        {
            get
            {
                if (Costo == null || TotalGasto == null || HacenPorMes == 0)
                    return 0;
                return (decimal)TotalGasto.TotalGastoFijo / HacenPorMes;
            }
        }

        public decimal CostoTotalCamion
        {
            get
            {
                if (Costo == null)
                    return 0;
                return (decimal)Costo.CostoPorCamion.Sum(c => c.Monto);
            }
        }

        public decimal CostoFinalPallet
        {
            get
            {
                if (Costo == null)
                    return 0;
                var costoFinal = (decimal)(CostoTotalCamion / Costo.CargaCamion + GastoDelPallet);
                /*
                if (Costo.PrecioPallet.HasValue)
                {
                    Costo.GananciaPorCantPallet = (int)Costo.PrecioPallet - (int)costoFinal;
                }*/

                return (decimal)(CostoTotalCamion / Costo.CargaCamion + GastoDelPallet);
            }
        }

        public decimal GananciaPorCamion
        {
            get
            {
                if (Costo == null || Costo.GananciaPorCantPallet == null)
                    return 0;
                return (decimal)Costo.GananciaPorCantPallet * Costo.CargaCamion;
            }
        }

    }
}
