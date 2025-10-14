using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class ActivoPasivoViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        public ObservableCollection<ActivoPasivo> listActivosPasivos = [];

        [ObservableProperty]
        public ObservableCollection<ActivoPasivo> listActivos = [];

        [ObservableProperty]
        public ObservableCollection<ActivoPasivo> listPasivos = [];

        [ObservableProperty]
        public ObservableCollection<QuincenaDTO> listQuinsenasActivos = [];

        [ObservableProperty]
        public ObservableCollection<QuincenaDTO> listQuinsenasPasivos = [];

        [ObservableProperty]
        public ObservableCollection<QuincenaNetoDTO> listQuincenasNetas = [];


        [ObservableProperty]
        public bool isBusy;

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

        public ActivoPasivoViewModel(ActivoPasivoController controller)
        {
            _activoPasivoController = controller;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        partial void OnMesIngresadoChanged(int value)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarActivosPasivos();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }

        partial void OnAñoIngresadoChanged(int value)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarActivosPasivos();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }


        public async Task CargarActivosPasivos()
        {
            try
            {
                IsBusy = true;
                var activosPasivos = await _activoPasivoController.GetAllActivosPasivos(new DateTime(AñoIngresado, MesIngresado + 1, 1));
                ListActivosPasivos = new ObservableCollection<ActivoPasivo>(activosPasivos);

                ListActivos = new ObservableCollection<ActivoPasivo>(ListActivosPasivos.Where(ap => ap.Categoria.Equals("Activo")).OrderBy(a => a.Fecha).ToList());
                ListPasivos = new ObservableCollection<ActivoPasivo>(ListActivosPasivos.Where(ap => ap.Categoria.Equals("Pasivo")).OrderBy(p => p.Fecha).ToList());

                // Activos agrupados por quincena
                var quincenasActivos = ListActivos
                    .GroupBy(a =>
                    {
                        var mes = a.Mes;
                        return (mes.Year, mes.Month, Quincena: mes.Day <= 15 ? 1 : 2);
                    })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ThenBy(g => g.Key.Quincena)
                    .Select(g => new QuincenaDTO
                    {
                        Año = g.Key.Year,
                        Mes = g.Key.Month,
                        Numero = g.Key.Quincena,
                        Items = new ObservableCollection<ActivoPasivo>(g.ToList())
                    });

                ListQuinsenasActivos = new ObservableCollection<QuincenaDTO>(quincenasActivos);

                // Pasivos agrupados por quincena
                var quincenasPasivos = ListPasivos
                    .GroupBy(p =>
                    {
                        var mes = p.Mes;
                        return (mes.Year, mes.Month, Quincena: mes.Day <= 15 ? 1 : 2);
                    })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ThenBy(g => g.Key.Quincena)
                    .Select(g => new QuincenaDTO
                    {
                        Año = g.Key.Year,
                        Mes = g.Key.Month,
                        Numero = g.Key.Quincena,
                        Items = new ObservableCollection<ActivoPasivo>(g.ToList())
                    });

                ListQuinsenasPasivos = new ObservableCollection<QuincenaDTO>(quincenasPasivos);

                // CALCULAR TOTALES NETOS POR QUINCENA
                CalcularTotalesNetosPorQuincena();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalcularTotalesNetosPorQuincena()
        {
            ListQuincenasNetas.Clear();

            // Combinar todas las quincenas únicas (de activos y pasivos)
            var todasQuincenas = ListQuinsenasActivos
                .Concat(ListQuinsenasPasivos)
                .GroupBy(q => new { q.Año, q.Mes, q.Numero })
                .Select(g => g.First())
                .OrderBy(q => q.Año)
                .ThenBy(q => q.Mes)
                .ThenBy(q => q.Numero);

            foreach (var quincena in todasQuincenas)
            {
                // Buscar activos para esta quincena
                var activosQuincena = ListQuinsenasActivos
                    .FirstOrDefault(q => q.Año == quincena.Año && q.Mes == quincena.Mes && q.Numero == quincena.Numero);

                // Buscar pasivos para esta quincena
                var pasivosQuincena = ListQuinsenasPasivos
                    .FirstOrDefault(q => q.Año == quincena.Año && q.Mes == quincena.Mes && q.Numero == quincena.Numero);

                decimal totalActivos = activosQuincena?.Total ?? 0;
                decimal totalPasivos = pasivosQuincena?.Total ?? 0;
                decimal totalNeto = totalActivos - totalPasivos;

                // Crear DTO para el total neto
                var quincenaNeto = new QuincenaNetoDTO
                {
                    Año = quincena.Año,
                    Mes = quincena.Mes,
                    Numero = quincena.Numero,
                    TotalActivos = totalActivos,
                    TotalPasivos = totalPasivos,
                    TotalNeto = totalNeto
                };

                ListQuincenasNetas.Add(quincenaNeto);
            }
        }

        [RelayCommand]
        public async Task EliminarActivoPasivo(ActivoPasivo activoPasivo)
        {
            if (activoPasivo == null)
                return;

            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }

            bool confirmar = await mainPage.DisplayAlert("Confirmar", $"¿Está seguro de que desea eliminar este {activoPasivo.Categoria}?", "Sí", "No");
            if (!confirmar)
                return;
            try
            {
                IsBusy = true;
                bool eliminado = await _activoPasivoController.DeleteActivoPasivo(activoPasivo.ActivoPasivoId);
                if (eliminado)
                {
                    await CargarActivosPasivos();
                    CalcularTotalesNetosPorQuincena();
                    await MostrarAlerta("Éxito", "El registro ha sido eliminado correctamente.");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el registro. Inténtalo de nuevo.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al eliminar el registro: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task ModificarActivoPasivo(ActivoPasivo activoPasivoModificar)
        {
            if (activoPasivoModificar == null)
                return;
            var navigationParams = new Dictionary<string, object>
            {
                { "ActivoPasivo", activoPasivoModificar }
            };
            await Shell.Current.GoToAsync(nameof(Views.ActivoPasivoModificarView), navigationParams);
        }

        [RelayCommand]
        async Task CrearActivoPasivo()
        {
            await Shell.Current.GoToAsync(nameof(Views.ActivoPasivoCrearView));
        }

        [RelayCommand]
        async Task CopiarPasivos()
        {
            try
            {
                if (ListPasivos.Count == 0)
                {
                    await MostrarAlerta("Error", "No hay pasivos para copiar.");
                    return;
                }

                // Preguntar al usuario si desea copiar el presupuesto
                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }
                bool confirmar = await mainPage.DisplayAlert("Confirmar", $"¿Desea copiar estos pasivos al mes {MesToCopy + 1}-{AñosCopy[AñoToCopyIndex]}?", "Sí", "No");
                if (!confirmar)
                    return;

                // verificar que AñoToCopyIndex tenga valor
                if (AñoToCopyIndex < 0 || AñoToCopyIndex >= Años.Count || MesToCopy < 0)
                {
                    await MostrarAlerta("Error", "Año o Mes inválido para copiar el presupuesto.");
                    return;
                }

                // agregar los egresos al mes seleccionado
                foreach (var pasivo in ListPasivos)
                {
                    var nuevoPasivo = new ActivoPasivo
                    {
                        Fecha = new DateTime(AñosCopy[AñoToCopyIndex], MesToCopy + 1, pasivo.Fecha?.Day ?? 1),
                        Mes = new DateTime(AñosCopy[AñoToCopyIndex], MesToCopy + 1, pasivo.Mes.Day),
                        Descripcion = pasivo.Descripcion,
                        Monto = pasivo.Monto,
                        Categoria = pasivo.Categoria,
                        Estado = pasivo.Estado
                    };
                    // Mirar si ya hay un pasivo con la misma descripción en el mes y año seleccionados
                    bool existe = await _activoPasivoController.ExistePasivoEnMes(nuevoPasivo.Descripcion, nuevoPasivo.Monto, nuevoPasivo.Mes);
                    if (!existe)
                    {
                        MessageResult response = await _activoPasivoController.CreateActivoPasivo(nuevoPasivo);
                        if (response.Title == MessageConstants.Titles.Error)
                        {
                            await MostrarAlerta("Error", "Error al copiar los gastos fijos.");
                            return;
                        }
                    }
                }

                await MostrarAlerta("Éxito", "Gastos fijos copiados correctamente.");
                await CargarActivosPasivos();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al copiar los pasivos: {ex.Message}");
            }
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
