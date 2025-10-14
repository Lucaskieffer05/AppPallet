using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class PalletCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------
        readonly PalletController _palletController;

        [ObservableProperty]
        public Pallet? palletCreated;



        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public PalletCrearViewModel(PalletController palletController)
        {
            _palletController = palletController;
            PalletCreated = new();
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
        async Task CrearPallet()
        {
            if (PalletCreated == null)
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                MessageResult resultado = await _palletController.CrearPallet(PalletCreated!);
                await MostrarAlerta(resultado.Title, resultado.Message);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }

            await VolverAtras();
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
