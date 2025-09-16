using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class PresupuestoMostrarViewModel : ObservableObject, IQueryAttributable
    {


        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly EmpresaController _empresaController;
        readonly GastosFijosController _gastosFijosController;
        readonly CostoPorPalletController _costoPorPalletController;

        public List<TotalGastoFijoPorMes> TotalGastoFijoPorMesList;


        [ObservableProperty]
        public Empresa? empresaSeleccionada;

        [ObservableProperty]
        public List<GastosYCostos> resultado;

        [ObservableProperty]
        public string titulo;


        // Inicializa los campos en el constructor para evitar el error CS8618
        public PresupuestoMostrarViewModel(EmpresaController empresaController, GastosFijosController gastosFijosController, CostoPorPalletController costoPorPalletController)
        {
            _empresaController = empresaController;
            Titulo = "Presupuesto";
            _gastosFijosController = gastosFijosController;
            _costoPorPalletController = costoPorPalletController;

            TotalGastoFijoPorMesList = [];
            Resultado = [];
        }



        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        private async Task RecargarResultado(int empresaId)
        {

            // Vuelve a cargar los datos actualizados
            EmpresaSeleccionada = await _empresaController.GetEmpresaById(empresaId);
            TotalGastoFijoPorMesList = await _gastosFijosController.GetTotalGastoFijoPorMes();

            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Error", "No se encontró la empresa seleccionada.");
                return;
            }
               

            Resultado = EmpresaSeleccionada.CostoPorPallets
                .Select(c => new GastosYCostos
                {
                    Costo = c,
                    TotalGasto = TotalGastoFijoPorMesList.FirstOrDefault(t => t.Mes.Month == c.Mes.Month)
                })
                .ToList();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("EmpresaId", out var id) && id is int empresaId)
            {
                await RecargarResultado(empresaId);
            }

        }

        [RelayCommand]
        public async Task MostrarPresupuesto(GastosYCostos gastosYCostos)
        {
            await Shell.Current.GoToAsync(
                nameof(PresupuestoModificarView),
                new Dictionary<string, object>
                {
                    { "gastosYCostos", gastosYCostos }
                });
        }

        [RelayCommand]
        public async Task GuardarPrecio(GastosYCostos gastosYCostos)
        {
            // guardar solo el precio del pallet
            if (gastosYCostos.Costo != null)
            {
                if(gastosYCostos.CostoFinalPallet > 0)
                {
                    gastosYCostos.Costo.GananciaPorCantPallet = (int?)(gastosYCostos.Costo.PrecioPallet - gastosYCostos.CostoFinalPallet);
                }
                bool response = await _costoPorPalletController.UpdateCostoPorPallet(gastosYCostos.Costo);
                if (!response)
                {
                    await MostrarAlerta("Error", "Ocurrió un error al guardar el precio del pallet.");
                    return;
                }
            }

            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Error", "No hay una empresa seleccionada.");
                return;
            }

            await RecargarResultado(EmpresaSeleccionada.EmpresaId);


        }

        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }


    }

    
}
