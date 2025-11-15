using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppPallet.ViewModels
{
    public partial class ChequeViewModel : ObservableObject, INotifyPropertyChanged
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly ChequeController _chequeController;

        private readonly IPopupService _popupService;

        [ObservableProperty]
        private ObservableCollection<Cheque> listaCheques = [];

        [ObservableProperty]
        public bool hayCheques;

        [ObservableProperty]
        public Cheque? chequeSeleccionado;

        [ObservableProperty]
        private bool isBusy;

        private bool _isLoading = false;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;

        // Propiedades para sumatorias por estado
        [ObservableProperty]
        private int cantidadSinEstado;

        [ObservableProperty]
        private decimal montoSinEstado;

        [ObservableProperty]
        private int cantidadPagado;

        [ObservableProperty]
        private decimal montoPagado;

        [ObservableProperty]
        private int cantidadVencido;

        [ObservableProperty]
        private decimal montoVencido;

        [ObservableProperty]
        private int cantidadPagoInmediato;

        [ObservableProperty]
        private decimal montoPagoInmediato;

        [ObservableProperty]
        private int cantidadProximoAPagar;

        [ObservableProperty]
        private decimal montoProximoAPagar;

        [ObservableProperty]
        private int cantidadTotal;

        [ObservableProperty]
        private decimal montoTotal;

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

        public ChequeViewModel(IPopupService popupService, ChequeController chequeController)

        {
            _popupService = popupService;
            _chequeController = chequeController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaCheques()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                IsBusy = true;

                ChequeSeleccionado = null;

                var chequesList = await _chequeController.GetAllCheques(new DateTime(AñoIngresado, MesIngresado + 1, 1));
                ListaCheques = new ObservableCollection<Cheque>(chequesList);
                HayCheques = ListaCheques.Count == 0;
                CalcularSumatorias();
            }
            finally
            {
                IsBusy = false;
                _isLoading = false;
            }
        }

        private void CalcularSumatorias()
        {
            CantidadSinEstado = 0;
            MontoSinEstado = 0;
            CantidadPagado = 0;
            MontoPagado = 0;
            CantidadVencido = 0;
            MontoVencido = 0;
            CantidadPagoInmediato = 0;
            MontoPagoInmediato = 0;
            CantidadProximoAPagar = 0;
            MontoProximoAPagar = 0;
            CantidadTotal = 0;
            MontoTotal = 0;

            foreach (var cheque in ListaCheques)
            {
                var estado = cheque.Estado ?? 0;
                var monto = cheque.Monto;

                CantidadTotal++;
                MontoTotal += monto;

                switch (estado)
                {
                    case 0:
                        CantidadSinEstado++;
                        MontoSinEstado += monto;
                        break;
                    case 1:
                        CantidadPagado++;
                        MontoPagado += monto;
                        break;
                    case 2:
                        CantidadVencido++;
                        MontoVencido += monto;
                        break;
                    case 3:
                        CantidadPagoInmediato++;
                        MontoPagoInmediato += monto;
                        break;
                    case 4:
                        CantidadProximoAPagar++;
                        MontoProximoAPagar += monto;
                        break;
                }
            }
        }

        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarListaCheques();
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
                    await CargarListaCheques();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }


        [RelayCommand]
        public async Task MostrarPopupCheque()
        {

            await DisplayPopupCrear();
        }

        [RelayCommand]
        public async Task MostrarPopupChequeModificar()
        {
            if (ChequeSeleccionado == null)
            {
                await MostrarAlerta("Atención", "Debe seleccionar un cheque para modificar.");
            }
            else
            {
                await DisplayPopupModificar();
            }
        }

        public async Task DisplayPopupCrear()
        {
            var popupResult = await _popupService.ShowPopupAsync<ChequeCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty);
            await CargarListaCheques();
        }

        public async Task DisplayPopupModificar()
        {
            if (ChequeSeleccionado == null)
                throw new InvalidOperationException("ChequeSeleccionado no puede ser nulo al modificar.");

            var queryAttributes = new Dictionary<string, object>
            {
                ["ChequeSeleccionado"] = ChequeSeleccionado
            };

            var popupResult = await _popupService.ShowPopupAsync<ChequeModificarViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);
            await CargarListaCheques();
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
