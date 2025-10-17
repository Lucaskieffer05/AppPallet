using AppPallet.Constants;
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

        readonly EgresoController _egresoController;

        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        public ObservableCollection<GastosFijos> listaGastosFijos = [];

        [ObservableProperty]
        public GastosFijos? gastoFijoSeleccionado;

        [ObservableProperty]
        private decimal totalGastosFijos;

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

        [ObservableProperty]
        private int mesToCopy = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoToCopyIndex = 1;

        public ObservableCollection<string> MesesCopy { get; } =
    [
        "01", "02", "03", "04", "05", "06",
                "07", "08", "09", "10", "11", "12"
    ];

        public ObservableCollection<int> AñosCopy
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

        public GastosFijosViewModel(IPopupService popupService, GastosFijosController gastosFijosController, ActivoPasivoController activoPasivoController, EgresoController egresoController)
        {
            _popupService = popupService;
            _gastosFijosController = gastosFijosController;
            _activoPasivoController = activoPasivoController;
            _egresoController = egresoController;
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
                TotalGastosFijos = ListaGastosFijos.Sum(g => g.Monto);
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
                    Monto           = gasto.Monto,
                    Mes             = new DateTime(AñosCopy[AñoToCopyIndex], MesToCopy + 1, 1)
                };
                MessageResult responseGasto = await _gastosFijosController.CreateGastoFijo(nuevoGastoFijo);
                if (responseGasto.Title == MessageConstants.Titles.Error)
                {
                    await MostrarAlerta(responseGasto.Title,responseGasto.Message);
                    return;
                }

                // Crear un Egreso asociado si flagEgreso es true
                if (flagEgreso)
                {
                    bool error = await CrearUnEgreso(nuevoGastoFijo);
                    if (!error)
                    {
                        await MostrarAlerta("Error", "Error al crear el egreso asociado.");
                        return;
                    }
                }
                // crear Pasivo asociado si FlasgPasivo es true
                if (flagPasivo)
                {
                    bool error = await CrearUnPasivo(nuevoGastoFijo);
                    if (!error)
                    {
                        await MostrarAlerta("Error", "Error al crear el pasivo asociado.");
                        return;
                    }
                }
            }

            await MostrarAlerta("Éxito", "Gastos fijos copiados correctamente.");
            await CargarListaGastosFijos();
        }

        public async Task<bool> CrearUnEgreso(GastosFijos nuevoGastoFijo)
        {
            var nuevoEgreso = new Egreso
            {
                Fecha = nuevoGastoFijo.Mes,
                Mes = new DateTime(nuevoGastoFijo.Mes.Year, nuevoGastoFijo.Mes.Month, 1),
                Monto = nuevoGastoFijo.Monto,
                DescripEgreso = nuevoGastoFijo.NombreGastoFijo,
                Comentario = "Gasto Fijo"
            };
            bool existe = await _egresoController.ExisteEgresoEnMes(nuevoEgreso.DescripEgreso, nuevoEgreso.Monto, nuevoEgreso.Mes);
            if (!existe)
            {
                MessageResult responseEgreso = await _egresoController.CreateEgreso(nuevoEgreso);
                if (responseEgreso.Title == MessageConstants.Titles.Error)
                {
                    await MostrarAlerta(responseEgreso.Title, responseEgreso.Message);
                    return false;
                }
                return true;
            }
            return true;
        }
        public async Task<bool> CrearUnPasivo(GastosFijos nuevoGastoFijo)
        {
            var nuevoPasivo = new ActivoPasivo
            {
                Fecha = nuevoGastoFijo.Mes,
                Mes = new DateTime(nuevoGastoFijo.Mes.Year, nuevoGastoFijo.Mes.Month, 1),
                Monto = nuevoGastoFijo.Monto,
                Descripcion = nuevoGastoFijo.NombreGastoFijo,
                Categoria = "Pasivo"
            };
            bool existe = await _activoPasivoController.ExistePasivoEnMes(nuevoPasivo.Descripcion, nuevoPasivo.Monto, nuevoPasivo.Mes);
            if (!existe)
            {
                MessageResult responsePasivo = await _activoPasivoController.CreateActivoPasivo(nuevoPasivo);
                if (responsePasivo.Title == MessageConstants.Titles.Error)
                {
                    await MostrarAlerta(responsePasivo.Title, responsePasivo.Message);
                    return false;
                }
                return true;
            }
            return true;
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
