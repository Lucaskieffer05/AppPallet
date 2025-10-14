using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;
using AppPallet.Constants;

namespace AppPallet.ViewModels
{
    public partial class EgresoCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private EgresoController _egresoController;

        [ObservableProperty]
        public Egreso egresoCreated;

        [ObservableProperty]
        private bool conIva;

        [ObservableProperty]
        public int mesIngresado;

        [ObservableProperty]
        public int añoIngresado;

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

        public EgresoCrearViewModel(EgresoController egresoController)
        {
            _egresoController = egresoController;
            EgresoCreated = new Egreso
            {
                Fecha = DateTime.Today
            };

            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;
            ConIva = false;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task VolverAtras()
        {
            try
            {
                // Forzar en el hilo principal
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("..");
                });
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error de navegación", ex.Message);
            }
        }

        [RelayCommand]
        async Task CrearEgreso()
        {
            if (EgresoCreated == null || !ValidarEgreso(EgresoCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                EgresoCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);
                if (ConIva && EgresoCreated.Monto <= 0)
                {
                    await MostrarAlerta("Error", "Debe ingresar un Monto si seleccionó que el egreso tiene IVA");
                    return;
                }
                double iva = Preferences.Get("IVA", 0.0);
                EgresoCreated.SumaIva = ConIva ? EgresoCreated.Monto * (decimal)iva : null;

                MessageResult resultado = await _egresoController.CreateEgreso(EgresoCreated);
                await MostrarAlerta(resultado.Title, resultado.Message);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }

            await VolverAtras();
        }

        [RelayCommand]
        void ToggleIva()
        {
            ConIva = !ConIva;
        }


        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }

        private bool ValidarEgreso(Egreso egreso)
        {
            if (egreso == null)
                return false;
            if (string.IsNullOrWhiteSpace(egreso.DescripEgreso))
                return false;
            if (egreso.Monto < 0) // Ejemplo de rango
                return false;
            if (egreso.Fecha > DateTime.Today)
                return false;
            return true;



        }
    }
}
