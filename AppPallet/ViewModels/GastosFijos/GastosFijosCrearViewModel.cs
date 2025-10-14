using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class GastosFijosCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        [ObservableProperty]
        public GastosFijos? gastosFijosCreated;

        readonly IPopupService _popupService;

        readonly GastosFijosController _gastosFijosController;
        readonly EgresoController _egresoController;
        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        private int mesIngresado;

        [ObservableProperty]
        private int añoIngresado;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public GastosFijosCrearViewModel(IPopupService popupService, GastosFijosController gastosFijosController, EgresoController egresoController, ActivoPasivoController activoPasivoController)
        {
            _popupService = popupService;
            _gastosFijosController = gastosFijosController;
            _egresoController = egresoController;
            _activoPasivoController = activoPasivoController;
            GastosFijosCreated = new GastosFijos();
            GastosFijosCreated.Mes = DateTime.Today;

            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CerrarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (GastosFijosCreated == null || !ValidarGastosFijos(GastosFijosCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {
                GastosFijosCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);

                MessageResult resultado = await _gastosFijosController.CreateGastoFijo(GastosFijosCreated);

                await MostrarAlerta(resultado.Title, resultado.Message);

                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }

                bool flagEgreso = await mainPage.DisplayAlert("Confirmar", $"¿Quieres agregar el gasto fijo creado como Egreso automaticamente?", "Sí", "No");
                bool flagPasivo = await mainPage.DisplayAlert("Confirmar", $"¿Desea también agregar el gasto fijo creado como Pasivos automaticamente?", "Sí", "No");

                if (flagEgreso)
                {
                    await CrearUnEgreso(GastosFijosCreated);
                }
                if (flagPasivo)
                {
                    await CrearUnPasivo(GastosFijosCreated);
                }

                GastosFijosCreated = new GastosFijos();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CerrarPopup();
        }

        bool ValidarGastosFijos(GastosFijos gastosFijos)
        {
            // Eliminar la comprobación de null para DateTime, ya que nunca puede ser null
            if (string.IsNullOrWhiteSpace(gastosFijos.NombreGastoFijo) || gastosFijos.Monto <= 0)
            {
                return false;
            }

            return true;
        }

        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }

        public async Task CrearUnEgreso(GastosFijos nuevoGastoFijo)
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
                    return;
                }
            }
        }

        public async Task CrearUnPasivo(GastosFijos nuevoGastoFijo)
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
                    return;
                }
            }
        }

    }
}
