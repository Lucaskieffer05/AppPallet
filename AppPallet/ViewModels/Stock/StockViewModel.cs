using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class StockViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly PalletController _palletController;

        [ObservableProperty]
        private ObservableCollection<PalletStockDTO> listPallets = [];

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private PalletStockDTO? palletSeleccionado;

        [ObservableProperty]
        private int cantidadAjuste = 1;

        // Propiedades calculadas para el footer
        [ObservableProperty]
        public int totalPallets;
        [ObservableProperty]
        public int totalStock;
        [ObservableProperty]
        public int palletsBajoStock;
        [ObservableProperty]
        public int palletsSinStock;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public StockViewModel(PalletController palletController)
        {
            _palletController = palletController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task LoadPallets()
        {
            try
            {
                IsBusy = true;
                var pallets = await _palletController.GetAllPallets();

                ListPallets = new ObservableCollection<PalletStockDTO>(
                    pallets.Select(p => new PalletStockDTO(p))
                );

                // Notificar cambios en las propiedades calculadas
                TotalPallets = ListPallets.Count;
                TotalStock = ListPallets.Sum(p => p.Stock);
                PalletsBajoStock = ListPallets.Count(p => p.Stock <= p.StockMinimo && 0 < p.Stock);
                PalletsSinStock = ListPallets.Count(p => p.Stock == 0);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudieron cargar los pallets. {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AjustarStock(PalletStockDTO pallet)
        {
            if (pallet == null || pallet.Stock < 0)
            {
                await MostrarAlerta("Error", "Seleccione un pallet y especifique una cantidad válida.");
                return;
            }

            try
            {
                IsBusy = true;
                var resultado = await _palletController.UpdatePallet(pallet);
                await MostrarAlerta(resultado.Title, resultado.Message);
                await LoadPallets();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al ajustar stock: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task VerDetallesPallet(PalletStockDTO pallet)
        {
            PalletSeleccionado = pallet;
            // Aquí podrías navegar a una vista de detalles si es necesario
            await MostrarAlerta("Detalles", $"{pallet.Nombre}\nStock: {pallet.Stock}\nDescripción: {pallet.Descripcion}");
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