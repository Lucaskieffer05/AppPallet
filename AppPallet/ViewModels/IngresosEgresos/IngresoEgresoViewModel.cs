using AppPallet.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class IngresoEgresoViewModel
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly EgresoController _egresoController;
        readonly IngresoController _ingresoController;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public IngresoEgresoViewModel(EgresoController egresoController, IngresoController ingresoController)
        {
            _egresoController = egresoController;
            _ingresoController = ingresoController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

    }
}
