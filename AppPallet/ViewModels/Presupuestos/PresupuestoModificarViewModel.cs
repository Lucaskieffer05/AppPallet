using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class PresupuestoModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly CostoPorPalletController _costoPorPalletController;
        readonly CostoPorCamionController _constoPorCamionController;

        [ObservableProperty]
        public CostoPorPallet? costoPorPalletSeleccionada;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public PresupuestoModificarViewModel(CostoPorPalletController costoPorPalletController, CostoPorCamionController costoPorCamionController)
        {
            _costoPorPalletController = costoPorPalletController;
            _constoPorCamionController = costoPorCamionController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("gastosYCostos", out var _gastosYCostos) && _gastosYCostos is GastosYCostos gastosYCostos)
            {
                if (gastosYCostos.Costo != null)
                {
                    CostoPorPalletSeleccionada = await _costoPorPalletController.GetCostoPorPalletById(gastosYCostos.Costo.CostoPorPalletId);
                    if (CostoPorPalletSeleccionada == null)
                    {
                        return;
                    }
                }
            }
        }

        [RelayCommand]
        public Task EliminarCostoPorCamion(CostoPorCamion costoPorCamion)
        {
            if (costoPorCamion == null)
            {
                return Task.CompletedTask;
            }

            CostoPorPalletSeleccionada?.CostoPorCamions.Remove(costoPorCamion);

            return Task.CompletedTask;
        }

        [RelayCommand]
        public Task AgregarCostoPorCamion()
        {
            CostoPorPalletSeleccionada?.CostoPorCamions.Add(new CostoPorCamion
            {
                NombreCosto = string.Empty,
                Monto = 0,
                CostoPorPalletId = CostoPorPalletSeleccionada.CostoPorPalletId
            });
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task GuardarCambios()
        {
            if (CostoPorPalletSeleccionada == null)
            {
                await MostrarAlerta("Error", "No hay un costo por pallet seleccionado.");
                return;
            }

            try
            {
                var filtro = CostoPorPalletSeleccionada.CostoPorCamions
                    .Where(c => !string.IsNullOrWhiteSpace(c.NombreCosto) && c.Monto > 0)
                    .ToList();

                CostoPorPalletSeleccionada.CostoPorCamions = new ObservableCollection<CostoPorCamion>(filtro);

                var respuesta = await _costoPorPalletController.UpdateCostoPorPallet(CostoPorPalletSeleccionada);
                if (!respuesta)
                {
                    await MostrarAlerta("Error", "Ocurrió un error al guardar los cambios.");
                    return;
                }
                await MostrarAlerta("Éxito", "Los cambios se han guardado correctamente.");

                //Volver atras
                await Shell.Current.GoToAsync("..");


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await MostrarAlerta("Error", "Ocurrió un error al guardar los cambios.");
            }
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
