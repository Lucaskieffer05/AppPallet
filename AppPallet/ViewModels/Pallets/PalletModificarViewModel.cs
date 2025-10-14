using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class PalletModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly PalletController _palletController;

        [ObservableProperty]
        public Pallet? palletModificar;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public PalletModificarViewModel(PalletController palletController)
        {
            _palletController = palletController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Pallet", out var pallet) && pallet is Pallet _pallet)
            {
                PalletModificar = _pallet;
            }
        }

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
        async Task GuardarCambios()
        {
            try
            {
                if (PalletModificar is null)
                {
                    await MostrarAlerta("Error", "No hay datos para guardar.");
                    return;
                }
                // Actualizar en la base de datos
                MessageResult respuesta = await _palletController.UpdatePallet(PalletModificar);
                
                await MostrarAlerta(respuesta.Title, respuesta.Message);

            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error al guardar los cambios: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task EliminarPallet()
        {
            try
            {
                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }


                if (PalletModificar is null)
                {
                    await MostrarAlerta("Error", "No hay datos para eliminar.");
                    return;
                }
                bool confirmar = await mainPage.DisplayAlert(
                    "Confirmar eliminación",
                    $"¿Estás seguro de que deseas eliminar el pallet '{PalletModificar.Nombre}'?",
                    "Sí",
                    "No");
                if (!confirmar)
                {
                    return; // El usuario canceló la eliminación
                }
                PalletModificar.FechaEliminacion = DateTime.Now;
                MessageResult respuesta = await _palletController.UpdatePallet(PalletModificar);
                await MostrarAlerta(respuesta.Title, respuesta.Message);
                await VolverAtras();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error al eliminar el pallet: {ex.Message}");
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
