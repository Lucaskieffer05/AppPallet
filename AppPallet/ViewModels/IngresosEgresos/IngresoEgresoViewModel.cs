using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class IngresoEgresoViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly EgresoController _egresoController;
        readonly IngresoController _ingresoController;

        [ObservableProperty]
        public decimal totalIngresos;

        [ObservableProperty]
        public decimal totalEgresos;

        [ObservableProperty]
        public decimal totalEgresosIva;

        [ObservableProperty]
        public bool isBusy;

        [ObservableProperty]
        public ObservableCollection<Egreso> listEgresos = [];

        [ObservableProperty]
        public ObservableCollection<Ingreso> listIngresos = [];


        [ObservableProperty]
        public Egreso? egresoSeleccionado;

        [ObservableProperty]
        public Ingreso? ingresoSeleccionado;


        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];

        private DateTime MesFiltro = DateTime.Now;



        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public IngresoEgresoViewModel(EgresoController egresoController, IngresoController ingresoController)
        {
            _egresoController = egresoController;
            _ingresoController = ingresoController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------


        public async Task CargarListaIngresosEgresos()
        {
            try
            {
                IsBusy = true;

                var _listEgresos = await _egresoController.GetAllEgresos(MesFiltro);
                ListEgresos = new ObservableCollection<Egreso>(_listEgresos);

                var _listIngresos = await _ingresoController.GetAllIngresos(MesFiltro);
                ListIngresos = new ObservableCollection<Ingreso>(_listIngresos);

                // calcular totales
                TotalEgresos = ListEgresos.Sum(e => e.Monto);
                TotalEgresosIva = ListEgresos.Sum(e => e.SumaIva ?? 0);
                TotalIngresos = ListIngresos.Sum(i => i.Monto);

            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task FiltrarPorMes()
        {
            MesFiltro = new DateTime(AñoIngresado, MesIngresado + 1, 1);
            await CargarListaIngresosEgresos();
        }


        [RelayCommand]
        async Task ModificarIngreso()
        {

            if (IngresoSeleccionado == null)
            {
                await MostrarAlerta("Error", "No hay ningún ingreso seleccionado");
                return;
            }

            var navigationParams = new Dictionary<string, object>
            {
                { "Ingreso", IngresoSeleccionado }
            };
            await Shell.Current.GoToAsync(nameof(IngresoModificarView), navigationParams);

        }

        [RelayCommand]
        async Task CrearIngreso()
        {
            await Shell.Current.GoToAsync(nameof(IngresoCrearView));
        }


        [RelayCommand]
        async Task ModificarEgreso()
        {
            if (EgresoSeleccionado == null)
            {
                await MostrarAlerta("Error", "No hay ningún egreso seleccionado");
                return;
            }
            var navigationParams = new Dictionary<string, object>
            {
                { "Egreso", EgresoSeleccionado }
            };
            await Shell.Current.GoToAsync(nameof(EgresoModificarView), navigationParams);
        }

        [RelayCommand]
        async Task CrearEgreso()
        {
            await Shell.Current.GoToAsync(nameof(EgresoCrearView));
        }




        #region Funciones Auxiliares


        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }


        #endregion

    }
}
