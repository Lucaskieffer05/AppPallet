using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class VentaViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly VentaController _ventaController;

        [ObservableProperty]
        private ObservableCollection<Venta> listaVentas = [];

        [ObservableProperty]
        public Venta? ventaSeleccionada;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;


        public ObservableCollection<string> Meses { get; } = new()
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

        public ObservableCollection<int> Años
        {
            get
            {
                var añoMinimo = Preferences.Get("año_minimo", DateTime.Now.Year - 1);
                var añoMaximo = Preferences.Get("año_maximo", DateTime.Now.Year + 1);

                return new ObservableCollection<int>(
                    Enumerable.Range(añoMinimo, añoMaximo - añoMinimo + 1)
                );
            }
        }

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
                var ventasList = await _ventaController.GetAllVentas(new DateTime(AñoIngresado, MesIngresado + 1, 1));
                ListaVentas = new ObservableCollection<Venta>(ventasList);
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

        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            _ = CargarListaVentas();
        }

        partial void OnAñoIngresadoChanged(int oldValue, int newValue)
        {
            _ = CargarListaVentas();
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
