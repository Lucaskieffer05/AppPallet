using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class GastosFijosModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        [ObservableProperty]
        public GastosFijos? gastosFijoSeleccionado;

        readonly IPopupService _popupService;

        readonly GastosFijosController _gastosFijoController;

        [ObservableProperty]
        private int mesIngresado;

        [ObservableProperty]
        private int añoIngresado;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------


        public GastosFijosModificarViewModel(IPopupService popupService, GastosFijosController gastosFijoController)
        {
            _popupService = popupService;
            _gastosFijoController = gastosFijoController;

            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CancelarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (GastosFijoSeleccionado == null || !ValidarGastosFijo())
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {
                GastosFijoSeleccionado.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);

                var resultado = await _gastosFijoController.UpdateGastoFijo(GastosFijoSeleccionado);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Gasto fijo actualizado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo actualizar el Gastos Fijo");
                }
                GastosFijoSeleccionado = new GastosFijos();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CancelarPopup();
        }

        [RelayCommand]
        async Task EliminarGastosFijo()
        {
            if (GastosFijoSeleccionado == null || GastosFijoSeleccionado.GastosFijosId == 0)
            {
                await MostrarAlerta("Error", "Datos inválidos");
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
            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar este Gastos Fijo?", "Sí", "No");
            if (!confirmar)
                return;

            try
            {
                var resultado = await _gastosFijoController.DeleteGastoFijo(GastosFijoSeleccionado.GastosFijosId);
                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Gasto Fijo eliminado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el Gasto Fijo");
                }
                GastosFijoSeleccionado = new GastosFijos();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }
            await CancelarPopup();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            GastosFijoSeleccionado = (GastosFijos)query["GastosFijoSeleccionado"];
        }

        private bool ValidarGastosFijo()
        {
            if (string.IsNullOrWhiteSpace(GastosFijoSeleccionado?.NombreGastoFijo) || GastosFijoSeleccionado.Monto <= 0)
            {
                return false;
            }
            return true;
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
