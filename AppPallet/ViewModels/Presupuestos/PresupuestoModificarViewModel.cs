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

        [ObservableProperty]
        public ObservableCollection<CostoPorCamion> listCostoPorCamions = [];

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
                    ListCostoPorCamions = new ObservableCollection<CostoPorCamion>(CostoPorPalletSeleccionada.CostoPorCamions);

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

            ListCostoPorCamions.Remove(costoPorCamion);

            return Task.CompletedTask;
        }

        [RelayCommand]
        public Task AgregarCostoPorCamion()
        {
            if (CostoPorPalletSeleccionada == null)
            {
                return Task.CompletedTask;
            }

            ListCostoPorCamions.Add(new CostoPorCamion
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
                var filtro = ListCostoPorCamions.Where(c => !string.IsNullOrWhiteSpace(c.NombreCosto) && c.Monto > 0).ToList();

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

        [RelayCommand]
        public async Task EliminarCostoPorPallet()
        {
            if (CostoPorPalletSeleccionada == null)
            {
                await MostrarAlerta("Error", "No hay un costo por pallet seleccionado.");
                return;
            }

            // Obtener la página principal de forma compatible con la nueva API
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }

            // Confirmar eliminación
            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar este presupuesto?. Los costos por camión de este presupuesto también serán eliminados", "Sí", "No");
            if (!confirmar)
                return;


            try
            {
                var respuesta = await _costoPorPalletController.DeleteCostoPorPallet(CostoPorPalletSeleccionada.CostoPorPalletId);
                if (!respuesta)
                {
                    await MostrarAlerta("Error", "Ocurrió un error al eliminar el costo por pallet.");
                    return;
                }
                await MostrarAlerta("Éxito", "El costo por pallet se ha eliminado correctamente.");
                //Volver atras
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await MostrarAlerta("Error", "Ocurrió un error al eliminar el costo por pallet.");
            }
        }




        [RelayCommand]
        public async Task CancelarCambios()
        {
            //Volver atras
            await Shell.Current.GoToAsync("..");
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
