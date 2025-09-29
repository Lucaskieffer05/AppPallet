using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppPallet.ViewModels
{
    public partial class GastosFijosViewModel : ObservableObject, INotifyPropertyChanged
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly IPopupService _popupService;

        readonly GastosFijosController _gastosFijosController;

        [ObservableProperty]
        public ObservableCollection<GastosFijos> listaGastosFijos = [];

        [ObservableProperty]
        public GastosFijos? gastoFijoSeleccionado;

        [ObservableProperty]
        public bool isBusy;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;


        public ObservableCollection<string> Meses { get; } = new()
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

        public ObservableCollection<int> Años { get; } = new()
            {
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            };


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public GastosFijosViewModel(IPopupService popupService, GastosFijosController gastosFijosController)
        {
            _popupService = popupService;
            _gastosFijosController = gastosFijosController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaGastosFijos()
        {

            try
            {
                IsBusy = true;

                GastoFijoSeleccionado = null;

                var gastosFijoList = await _gastosFijosController.GetAllGastosFijos(new DateTime(AñoIngresado, MesIngresado + 1, 1));
                ListaGastosFijos = new ObservableCollection<GastosFijos>(gastosFijoList);
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarListaGastosFijos();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }

        partial void OnAñoIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarListaGastosFijos();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }

        [RelayCommand]
        public async Task MostrarPopupGastoFijo()
        {
            await DisplayPopupCrear();
        }

        [RelayCommand]
        public async Task MostrarPopupModificarGastoFijo()
        {
            if (GastoFijoSeleccionado == null)
            {
                await MostrarAlerta("Error", "Seleccione un gasto fijo para modificar.");
                return;
            }
            await DisplayPopupModificar();
        }

        public async Task DisplayPopupCrear()
        {
            var popupResult = await _popupService.ShowPopupAsync<GastosFijosCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty);

        }

        public async Task DisplayPopupModificar()
        {
            if (GastoFijoSeleccionado == null)
                throw new InvalidOperationException("El Gasto no puede ser nulo al modificar.");

            var queryAttributes = new Dictionary<string, object>
            {
                ["GastosFijoSeleccionado"] = GastoFijoSeleccionado
            };

            var popupResult = await _popupService.ShowPopupAsync<GastosFijosModificarViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);
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
