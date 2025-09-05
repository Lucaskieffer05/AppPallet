using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class PresupuestoViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly EmpresaController _empresaController;

        [ObservableProperty]
        public ObservableCollection<Empresa> listaEmpresas = [];

        [ObservableProperty]
        public Empresa? empresaSeleccionada;

        [ObservableProperty]
        private bool isBusy;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public PresupuestoViewModel(EmpresaController empresaController)
        {
            _empresaController = empresaController;
        }
        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        //Obtener empresas
        public async Task CargarListaEmpresas()
        {
            try
            {
                IsBusy = true;
                var lista = await _empresaController.GetAllEmpresas();
                ListaEmpresas = new ObservableCollection<Empresa>(lista);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
