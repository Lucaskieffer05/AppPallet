using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class VentaCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly VentaController _ventaController;

        [ObservableProperty]
        private Venta ventaCreated;

        [ObservableProperty]
        private string estadoingresado;

        public ObservableCollection<string> Estados { get; } =
        [
            "En Producción", "Entregado"
        ];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaCrearViewModel(VentaController ventaController)
        {
            _ventaController = ventaController;
            Estadoingresado = "En Producción";
            VentaCreated = new Venta
            {
                FechaVenta = DateTime.Now,
                CantPallets = 1,
                Estado = "En Producción",
                CostoPorPalletId = 0,
                Comentario = string.Empty,
                FechaEntrega = null
            };
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task VolverAtras()
        {
            //Volver atras
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task CrearVenta()
        {
            VentaCreated.Estado = Estadoingresado;
            if (!ValidarVenta(VentaCreated))
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos obligatorios.");
                return;
            }
            MessageResult resultado = await _ventaController.CreateVenta(VentaCreated);
            
            await MostrarAlerta(resultado.Title, resultado.Message);

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


        private bool ValidarVenta(Venta venta)
        {
            if (venta.CantPallets <= 0)
                return false;
            if (venta.FechaVenta == default)
                return false;
            if (string.IsNullOrWhiteSpace(venta.Estado))
                return false;
            if (venta.CostoPorPalletId <= 0)
                return false;
            return true;

        }


    }
}
