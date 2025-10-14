using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class ActivoPasivoModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        public ActivoPasivo? activoPasivoModified;

        [ObservableProperty]
        public string? tituloModificar;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;

        [ObservableProperty]
        private int quincenaIngresada = 0;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];

        public ObservableCollection<string> Quincenas { get; } =
            [
                "Primera Quincena", "Segunda Quincena"
            ];

        [ObservableProperty]
        private string? estadoSeleccionado;

        public ObservableCollection<string> Estados { get; } =
            [
                "Sin Estado","Pagado", "Sin Pagar"
            ];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ActivoPasivoModificarViewModel(ActivoPasivoController activoPasivoController)
        {
            _activoPasivoController = activoPasivoController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ActivoPasivo", out var activoPasivo) && activoPasivo is ActivoPasivo _activoPasivo)
            {
                ActivoPasivoModified = _activoPasivo;
                MesIngresado = ActivoPasivoModified.Mes.Month - 1;
                AñoIngresado = ActivoPasivoModified.Mes.Year;
                QuincenaIngresada = ActivoPasivoModified.Mes.Day <= 15 ? 0 : 1;
                TituloModificar = $"Modificar el {ActivoPasivoModified?.Categoria} {ActivoPasivoModified?.Descripcion}";
                EstadoSeleccionado = ActivoPasivoModified?.Estado ?? "Sin Estado";
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
        async Task ModificarActivoPasivo()
        {
            if (ActivoPasivoModified == null || !ValidarActivoPasivo(ActivoPasivoModified))
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos correctamente.");
                return;
            }
            if (EstadoSeleccionado == "Sin Estado")
            {
                ActivoPasivoModified.Estado = null;
            }
            else
            {
                ActivoPasivoModified.Estado = EstadoSeleccionado;
            }
            if (QuincenaIngresada == 0)
            {
                ActivoPasivoModified.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);
            }
            else
            {
                ActivoPasivoModified.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 16);
            }
            MessageResult result = await _activoPasivoController.UpdateActivoPasivo(ActivoPasivoModified);

            await MostrarAlerta(result.Title, result.Message);

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

        private bool ValidarActivoPasivo(ActivoPasivo activoPasivo)
        {
            if (string.IsNullOrWhiteSpace(activoPasivo.Descripcion))
                return false;
            if (activoPasivo.Monto < 0)
                return false;
            if (string.IsNullOrWhiteSpace(activoPasivo.Categoria))
                return false;
            if (activoPasivo.Mes == default)
                return false;
            return true;
        }


    }
}
