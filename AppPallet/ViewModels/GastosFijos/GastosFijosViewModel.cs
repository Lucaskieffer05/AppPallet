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

        [ObservableProperty]
        private int mesToCopy = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoToCopyIndex = 1;

        public ObservableCollection<string> MesesCopy { get; } =
    [
        "01", "02", "03", "04", "05", "06",
                "07", "08", "09", "10", "11", "12"
    ];

        public ObservableCollection<int> AñosCopy { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];


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

                if (MesIngresado == -1) return;

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

        [RelayCommand]
        public async Task CopiarGastosFijos()
        {
            if (ListaGastosFijos.Count == 0)
            {
                await MostrarAlerta("Error", "No hay un costo para copiar.");
                return;
            }

            // Preguntar al usuario si desea copiar el presupuesto
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }
            bool confirmar = await mainPage.DisplayAlert("Confirmar", $"¿Desea copiar estos gastos fijos al mes {MesToCopy + 1}-{AñosCopy[AñoToCopyIndex]}?", "Sí", "No");
            if (!confirmar)
                return;

            // verificar que AñoToCopyIndex tenga valor
            if (AñoToCopyIndex < 0 || AñoToCopyIndex >= Años.Count || MesToCopy < 0)
            {
                await MostrarAlerta("Error", "Año o Mes inválido para copiar el presupuesto.");
                return;
            }

            bool flagEgreso = await mainPage.DisplayAlert("Confirmar", $"¿Quieres agregar los gastos fijos como Egresos automaticamente?", "Sí", "No");
            bool flagPasivo = await mainPage.DisplayAlert("Confirmar", $"¿Quieres agregar los gastos fijos como Pasivos automaticamente?", "Sí", "No");

            // agregar los gastos fijos al mes seleccionado
            foreach (var gasto in ListaGastosFijos)
            {
                var nuevoGastoFijo = new GastosFijos
                {
                    NombreGastoFijo = gasto.NombreGastoFijo,
                    Monto = gasto.Monto,
                    Mes = new DateTime(AñosCopy[AñoToCopyIndex], MesToCopy + 1, 1)
                };
                bool response = await _gastosFijosController.CreateGastoFijo(nuevoGastoFijo, flagEgreso, flagPasivo);
                if (!response)
                {
                    await MostrarAlerta("Error", "Error al copiar los gastos fijos.");
                    return;
                }
            }

            await MostrarAlerta("Éxito", "Gastos fijos copiados correctamente.");
            await CargarListaGastosFijos();
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
