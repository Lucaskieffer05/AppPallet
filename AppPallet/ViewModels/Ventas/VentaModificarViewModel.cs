using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class VentaModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly VentaController _ventaController;

        [ObservableProperty]
        public Venta? ventaModified;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaModificarViewModel(VentaController ventaController)
        {
            _ventaController = ventaController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("VentaId", out var ventaId) && ventaId is int _ventaId)
            {
                _ = CargarVentaAsync(_ventaId);

            }
        }

        private async Task CargarVentaAsync(int ventaId)
        {
            VentaModified = await _ventaController.GetVentaById(ventaId);

            if (VentaModified == null)
            {
                await MostrarAlerta("Venta Cargada", $"Venta ID: {ventaId} no fue cargada correctamente.");
            }
        }


        [RelayCommand]
        public async Task VolverAtras()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task ModificarVenta()
        {
            if (VentaModified != null && ValidarVenta(VentaModified))
            {
                MessageResult result = await _ventaController.UpdateVenta(VentaModified);

                await MostrarAlerta(result.Title, result.Message);
                await VolverAtras();
            }
            else
            {
                await MostrarAlerta("Error", "No hay datos de venta para modificar.");
            }
        }

        [RelayCommand]
        public async Task EliminarVenta()
        {
            if (VentaModified != null)
            {
                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }

                bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar esta Venta?", "Sí", "No");

                if (confirmar)
                {
                    MessageResult result = await _ventaController.DeleteVenta(VentaModified.VentaId);
                    await MostrarAlerta(result.Title, result.Message);
                    await VolverAtras();
                }
            }
            else
            {
                await MostrarAlerta("Error", "No hay datos de venta para eliminar.");
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
