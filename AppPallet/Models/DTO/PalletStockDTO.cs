using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Models
{
    public class PalletStockDTO : Pallet
    {
        public int StockMinimo => 10; // Puedes hacer esto configurable
        public string EstadoStock
        {
            get
            {
                if (Stock == 0) return "Sin Stock";
                if (Stock <= StockMinimo && 0 < Stock) return "Bajo Stock";
                return "Disponible";
            }
        }

        public string EstadoColor
        {
            get
            {
                if (Stock == 0) return "#F44336"; // Rojo
                if (Stock <= StockMinimo && 0 < Stock) return "#FF9800"; // Naranja
                return "#4CAF50"; // Verde
            }
        }

        public bool EstadoMal
        {
            get
            {
                if (Stock == 0) return true;
                return false;
            }
        }
        public bool EstadoAlert
        {
            get
            {
                if (Stock <= StockMinimo && 0 < Stock) return true;
                return false;
            }
        }
        public bool EstadoBien
        {
            get
            {
                if (!EstadoMal && !EstadoAlert) return true;
                return false;
            }
        }

        public PalletStockDTO() { }

        public PalletStockDTO(Pallet pallet)
        {
            PalletId = pallet.PalletId;
            Nombre = pallet.Nombre;
            Descripcion = pallet.Descripcion;
            Estructura = pallet.Estructura;
            Tratamiento = pallet.Tratamiento;
            Sello = pallet.Sello;
            Peso = pallet.Peso;
            ToleranciaPeso = pallet.ToleranciaPeso;
            FechaCreacion = pallet.FechaCreacion;
            FechaModificacion = pallet.FechaModificacion;
            Stock = pallet.Stock;
        }
    }
}
