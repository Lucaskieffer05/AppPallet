using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class LoteCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        readonly LoteController _loteController;

        [ObservableProperty]
        public Lote loteCreated;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public LoteCrearViewModel(LoteController loteController)
        {
            _loteController = loteController;
            LoteCreated = new();

            LoteCreated.FechaSolicitada = DateTime.Today;

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
        async Task CrearLote()
        {
            if (LoteCreated == null || !ValidarLote(LoteCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {

                var resultado = await _loteController.CreateLote(LoteCreated);
                await MostrarAlerta(resultado.Title, resultado.Message);

                if (resultado.Title == MessageConstants.Titles.Success)
                {
                    await VolverAtras();
                }
                return;

            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
                return;

            }

        }


        private bool ValidarLote(Lote lote)
        {
            if (string.IsNullOrWhiteSpace(lote.NomProveedor)) return false;
            if (lote.FechaSolicitada == default) return false;
            if (string.IsNullOrWhiteSpace(lote.NomCamionero)) return false;
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
