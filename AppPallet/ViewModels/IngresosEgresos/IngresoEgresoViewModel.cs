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
        public decimal totalIngresosIva;

        [ObservableProperty]
        public decimal totalEgresos;

        [ObservableProperty]
        public decimal totalEgresosIva;

        [ObservableProperty]
        public decimal neto;

        [ObservableProperty]
        public decimal netoIVA;

        [ObservableProperty]
        public decimal netoMasIVA;

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

        public IngresoEgresoViewModel(EgresoController egresoController, IngresoController ingresoController)
        {
            _egresoController = egresoController;
            _ingresoController = ingresoController;

            // Guardar un valor ELIMINARRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR
            Preferences.Set("IVA", 0.21);

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------


        public async Task CargarListaIngresosEgresos()
        {
            try
            {
                MesFiltro = new DateTime(AñoIngresado, MesIngresado + 1, 1);
                IsBusy = true;

                var _listEgresos = await _egresoController.GetAllEgresos(MesFiltro);
                ListEgresos = new ObservableCollection<Egreso>(_listEgresos);

                var _listIngresos = await _ingresoController.GetAllIngresos(MesFiltro);
                ListIngresos = new ObservableCollection<Ingreso>(_listIngresos);

                // calcular totales
                TotalEgresos = ListEgresos.Sum(e => e.Monto);
                TotalEgresosIva = ListEgresos.Sum(e => e.SumaIva ?? 0);
                TotalIngresos = ListIngresos.Sum(i => i.Monto);
                double iva = Preferences.Get("IVA", 0.0);
                TotalIngresosIva = TotalIngresos * (decimal)iva;
                Neto = TotalIngresos - TotalEgresos;
                NetoMasIVA = (TotalIngresos + TotalIngresosIva) - (TotalEgresos + TotalEgresosIva);
                NetoIVA = TotalIngresosIva - TotalEgresosIva;

            }
            catch(Exception e)
            {
                await MostrarAlerta("Error", $"Error al cargar la lista de ingresos y egresos: {e.Message}");

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
                    await CargarListaIngresosEgresos();
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
                    await CargarListaIngresosEgresos();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
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

        [RelayCommand]
        public async Task CopiarEgresos()
        {
            if (ListEgresos.Count == 0)
            {
                await MostrarAlerta("Error", "No hay un costo para copiar.");
                return;
            }

            // Elimina de la lista los egresos que son "Gastos Fijos"
            var gastosFijos = ListEgresos.Where(e => e.Comentario != null && e.Comentario.Contains("Gasto Fijo")).ToList();
            foreach (var gastoFijo in gastosFijos)
            {
                ListEgresos.Remove(gastoFijo);
            }

            // Preguntar al usuario si desea copiar el presupuesto
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }
            bool confirmar = await mainPage.DisplayAlert("Confirmar", $"¿Desea copiar estos egresos al mes {MesToCopy + 1}-{AñosCopy[AñoToCopyIndex]}? (Solo los egresos 'Gastos Fijos' no serán copiados)", "Sí", "No");
            if (!confirmar)
                return;

            // verificar que AñoToCopyIndex tenga valor
            if (AñoToCopyIndex < 0 || AñoToCopyIndex >= Años.Count || MesToCopy < 0)
            {
                await MostrarAlerta("Error", "Año o Mes inválido para copiar el presupuesto.");
                return;
            }

            // agregar los egresos al mes seleccionado
            foreach (var gasto in ListEgresos)
            {
                var nuevoEgreso = new Egreso
                {
                    DescripEgreso = gasto.DescripEgreso,
                    Fecha = gasto.Fecha,
                    Factura = gasto.Factura,
                    Monto = gasto.Monto,
                    SumaIva = gasto.SumaIva,
                    Mes = new DateTime(AñosCopy[AñoToCopyIndex], MesToCopy + 1, 1),
                    Comentario = gasto.Comentario
                };
                bool response = await _egresoController.CreateEgreso(nuevoEgreso);
                if (!response)
                {
                    await MostrarAlerta("Error", "Error al copiar los gastos fijos.");
                    return;
                }
            }

            await MostrarAlerta("Éxito", "Gastos fijos copiados correctamente.");
            await CargarListaIngresosEgresos();
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
