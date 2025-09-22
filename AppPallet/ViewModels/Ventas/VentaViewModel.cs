using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class VentaViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly VentaController _ventaController;

        [ObservableProperty]
        private List<Venta> listaVentas = [];

        [ObservableProperty]
        public Venta? ventaSeleccionada;

        [ObservableProperty]
        private bool isBusy;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaViewModel(VentaController ventaController)
        {
            _ventaController = ventaController;
            ventaSeleccionada = new Venta();
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaVentas()
        {
            try
            {
                IsBusy = true;
                VentaSeleccionada = null;
                var ventasList = await _ventaController.GetAllVentas();
                ListaVentas = ventasList;
            }
            catch (Exception ex)
            {
                // Manejar la excepción (por ejemplo, mostrar un mensaje de error)
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ModificarVenta(int ventaid)
        {
            if (ventaid <= 0)
            {
                await MostrarAlerta("Error", "No se ha seleccionado ninguna venta.");
                return;
            }
            var navigationParams = new Dictionary<string, object>
            {
                {"VentaId", ventaid}
            };
            await Shell.Current.GoToAsync(nameof(VentaModificarView), navigationParams);
        }

        [RelayCommand]
        async Task CrearVenta()
        {
            await Shell.Current.GoToAsync(nameof(VentaCrearView));
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
