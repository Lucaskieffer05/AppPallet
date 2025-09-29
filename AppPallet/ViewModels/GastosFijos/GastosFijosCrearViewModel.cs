using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class GastosFijosCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        [ObservableProperty]
        public GastosFijos? gastosFijosCreated;

        readonly IPopupService _popupService;

        readonly GastosFijosController _gastosFijosController;

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

        public GastosFijosCrearViewModel(IPopupService popupService, GastosFijosController gastosFijosController)
        {
            _popupService = popupService;
            _gastosFijosController = gastosFijosController;
            GastosFijosCreated = new GastosFijos();
            GastosFijosCreated.Mes = DateTime.Today;

            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CerrarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (GastosFijosCreated == null || !ValidarGastosFijos(GastosFijosCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {
                GastosFijosCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);

                var resultado = await _gastosFijosController.CreateGastoFijo(GastosFijosCreated);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "GastosFijos creado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo crear el GastosFijos");
                }
                GastosFijosCreated = new GastosFijos();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CerrarPopup();
        }

        bool ValidarGastosFijos(GastosFijos gastosFijos)
        {
            // Eliminar la comprobación de null para DateTime, ya que nunca puede ser null
            if (string.IsNullOrWhiteSpace(gastosFijos.NombreGastoFijo) || gastosFijos.Monto <= 0)
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
