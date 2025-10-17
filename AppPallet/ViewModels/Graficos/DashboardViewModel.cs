using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly VentaController _ventaController;
        private readonly IngresoController _ingresoController;
        private readonly EgresoController _egresoController;
        private readonly PedidoController _pedidoController;
        private readonly EmpresaController _empresaController;
        private readonly PalletController _palletController;
        private readonly GastosFijosController _gastosFijosController;
        private readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private decimal ingresosMesActual;

        [ObservableProperty]
        private decimal egresosMesActual;

        [ObservableProperty]
        private decimal balanceMesActual;

        [ObservableProperty]
        private int totalPalletsVendidos;

        [ObservableProperty]
        private int totalPedidosActivos;

        [ObservableProperty]
        private int totalClientesActivos;

        [ObservableProperty]
        private int stockTotalPallets;

        [ObservableProperty]
        private ObservableCollection<VentaMesDTO> ventasMensuales = [];

        [ObservableProperty]
        private ObservableCollection<FinanzaMensualDTO> ingresosMensuales = [];
        [ObservableProperty]
        private ObservableCollection<FinanzaMensualDTO> egresosMensuales = [];

        [ObservableProperty]
        private ObservableCollection<Pallet> stockPallets = [];

        [ObservableProperty]
        private ObservableCollection<ActivoPasivoMensualDTO> activoPasivoAnual = [];

        [ObservableProperty]
        private ObservableCollection<PedidoPendienteDTO> pedidosPendientes = [];

        [ObservableProperty]
        private ObservableCollection<GastosFijos> gastosFijos = [];

        // Propiedades para los gráficos de Microcharts
        [ObservableProperty]
        private Chart ventasChart = null!;

        [ObservableProperty]
        private Chart finanzasChart = null!;


        [ObservableProperty]
        private Chart egresosChart = null!;

        [ObservableProperty]
        private Chart stockChart = null!;

        [ObservableProperty]
        private Chart activo = null!;

        [ObservableProperty]
        private Chart pasivo = null!;

        [ObservableProperty]
        private Chart activoPasivo = null!;


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


        public DashboardViewModel(
            VentaController ventaController,
            IngresoController ingresoController,
            EgresoController egresoController,
            PedidoController pedidoController,
            EmpresaController empresaController,
            GastosFijosController gastosFijosController,
            PalletController palletController,
            ActivoPasivoController activoPasivoController)
        {
            _ventaController = ventaController;
            _ingresoController = ingresoController;
            _egresoController = egresoController;
            _pedidoController = pedidoController;
            _empresaController = empresaController;
            _palletController = palletController;
            _gastosFijosController = gastosFijosController;
            _activoPasivoController = activoPasivoController;

            // Inicializar gráficos vacíos

        }

        [RelayCommand]
        public async Task CargarDashboard()
        {
            try
            {
                IsBusy = true;


                // cargar todos los datos necesarios
                await CargarEstadisticasGenerales();
                await CargarVentasMensuales();
                await CargarFinanzasMensuales();
                await CargarStockPallets();
                await CargarPedidosPendientes();
                await CargarActivoPasivoAnuales();
                await CargarGastosFijos();
                await CargarEstadisticasGenerales();


                // Crear gráficos con los datos cargados
                CrearGraficos();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CrearGraficos()
        {
            #region 1. Gráfico de Ventas Mensuales (BarChart)
            var ventasEntries = new List<ChartEntry>();
            foreach (var venta in VentasMensuales)
            {
                ventasEntries.Add(new ChartEntry((float)venta.TotalVentas)
                {
                    Label = Meses[venta.Mes - 1],
                    ValueLabel = "$" + venta.TotalVentas.ToString("N0"),
                    Color = SKColor.Parse("#4CAF50"), // Verde
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });
            }
            VentasChart = new BarChart
            {
                Entries = ventasEntries,

                AnimationDuration = TimeSpan.FromSeconds(3),
                AnimationProgress = 0,
                LabelTextSize = 12,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Vertical
            };
            #endregion

            #region 2. Gráfico de ingresos
            var ingresosEntries = new List<ChartEntry>();
            foreach (var finanza in IngresosMensuales)
            {
                ingresosEntries.Add(new ChartEntry((float)finanza.TotalFinanza)
                {
                    Label = Meses[finanza.Mes - 1],
                    ValueLabel = "$" + finanza.TotalFinanza.ToString("N0"),
                    Color = SKColor.Parse("#52D660"), // Azul
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });
            }
            FinanzasChart = new LineChart
            {
                Entries = ingresosEntries,
                AnimationDuration = TimeSpan.FromSeconds(3),
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                LabelTextSize = 12,
                LineSize = 4,
                PointSize = 10
            };

            #endregion

            #region 3. Gráfico de Egresos
            var egresosEntries = new List<ChartEntry>();
            foreach (var egreso in EgresosMensuales)
            {
                egresosEntries.Add(new ChartEntry((float)egreso.TotalFinanza)
                {
                    Label = Meses[egreso.Mes - 1],
                    ValueLabel = "$" + egreso.TotalFinanza.ToString("N0"),
                    Color = SKColor.Parse("#F44336"), // Rojo
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });
            }
            // Puedes agregar otra propiedad ObservableProperty<Chart> para EgresosChart
            EgresosChart = new LineChart
            {
                Entries = egresosEntries,
                AnimationDuration = TimeSpan.FromSeconds(3),
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                LabelTextSize = 12,
                LineSize = 4,
                PointSize = 10
            };
            #endregion

            #region 4. Gráfico de Activos y Pasivos
            var activoEntries = new List<ChartEntry>();
            var pasivoEntries = new List<ChartEntry>();
            var activoPasivoEntries = new List<ChartEntry>();

            foreach (var item in ActivoPasivoAnual)
            {
                activoPasivoEntries.Add(new ChartEntry((float)item.TotalCapitalNeta)
                {
                    Label = Meses[item.Mes - 1],
                    ValueLabel = "$" + item.TotalCapitalNeta.ToString("N0"), 
                    Color = SKColor.Parse("#52A7D6"), 
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });

                /*
                // Entries para Activos (sin ValueLabel)
                activoEntries.Add(new ChartEntry((float)item.TotalActivo)
                {
                    Label = Meses[item.Mes - 1],
                    ValueLabel = "$" + item.TotalCapitalNeta.ToString("N0"), // Sin etiqueta de valor
                    Color = SKColor.Parse("#4CAF50"), // Verde para activos
                    TextColor = SKColor.Parse("#E1E1E1"),
                    ValueLabelColor = SKColor.Parse("#E1E1E1")
                });

                // Entries para Pasivos (con ValueLabel mostrando la diferencia neta)
                pasivoEntries.Add(new ChartEntry((float)item.TotalPasivo)
                {
                    Label = Meses[item.Mes - 1],
                    ValueLabel = "$" + item.TotalCapitalNeta.ToString("N0"), // Diferencia neta
                    Color = SKColor.Parse("#F44336"), // Rojo para pasivos
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });
                */
            }

            // Gráficos
            ActivoPasivo = new LineChart
            {
                Entries = activoPasivoEntries,
                AnimationDuration = TimeSpan.FromSeconds(3),
                LineMode = LineMode.Spline,
                EnableYFadeOutGradient = true,
                LabelTextSize = 12,
                PointMode = PointMode.Circle,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal
            };


            /*
            Activo = new LineChart
            {
                Entries = activoEntries,
                AnimationDuration = TimeSpan.FromSeconds(0),
                AnimationProgress = 1,
                LabelTextSize = 12,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal
            };

            // Gráfico de Pasivos 
            Pasivo = new LineChart
            {
                Entries = pasivoEntries,
                BackgroundColor = SKColors.Transparent,
                AnimationDuration = TimeSpan.FromSeconds(0),
                AnimationProgress = 1,
                LabelTextSize = 12,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal
            };
            */

            #endregion

            #region 5. Gráfico de Stock (DonutChart)
            var stockEntries = new List<ChartEntry>();
            var colors = new[]
            {
                SKColor.Parse("#4CAF50"), // Verde
                SKColor.Parse("#2196F3"), // Azul
                SKColor.Parse("#FF9800"), // Naranja
                SKColor.Parse("#9C27B0"), // Púrpura
                SKColor.Parse("#607D8B"), // Gris azulado
                SKColor.Parse("#795548")  // Marrón
            };

            for (int i = 0; i < StockPallets.Count; i++)
            {
                var pallet = StockPallets[i];
                stockEntries.Add(new ChartEntry(pallet.Stock)
                {
                    Label = pallet.Nombre,
                    ValueLabel = pallet.Stock.ToString(),
                    Color = colors[i % colors.Length],
                    TextColor = SKColor.Parse("#374151"),
                    ValueLabelColor = SKColor.Parse("#374151")
                });
            }

            StockChart = new DonutChart
            {
                Entries = stockEntries,
                AnimationDuration = TimeSpan.FromSeconds(3),
                LabelTextSize = 12,
                GraphPosition = GraphPosition.Center,
                HoleRadius = 0.4f
            };
            #endregion


        }

        // Métodos auxiliares para cargar datos reales (cuando los tengas implementados)
        private async Task CargarEstadisticasGenerales()
        {
            try
            {

                // Ejemplo de cómo cargar datos reales (descomenta cuando tengas los métodos)
                DateTime FechaIngresada = new DateTime(AñoIngresado, MesIngresado + 1, 1);

                var ingresos = await _ingresoController.GetAllIngresos(FechaIngresada);
                var egresos = await _egresoController.GetAllEgresos(FechaIngresada);

                IngresosMesActual = ingresos.Sum(i => i.Monto);
                EgresosMesActual = egresos.Sum(e => e.Monto);
                BalanceMesActual = IngresosMesActual - EgresosMesActual;

                var ventasMes = await _ventaController.GetAllVentas(FechaIngresada);
                TotalPalletsVendidos = ventasMes.Sum(v => v.CantPallets);

                var pedidosActivos = await _pedidoController.GetAllPedidos(FechaIngresada);
                TotalPedidosActivos = pedidosActivos.Count;

                var clientes = await _empresaController.GetAllEmpresas("Cliente");
                TotalClientesActivos = clientes.Count;

                var pallets = await _palletController.GetAllPallets();
                StockTotalPallets = pallets.Sum(p => p.Stock);

            }
            catch (Exception ex)
            {
                // Manejar error
                Console.WriteLine($"Error cargando estadísticas: {ex.Message}");
            }
        }

        private async Task CargarVentasMensuales()
        {
            // Ejemplo de implementación real
            DateTime FechaIngresada = new(AñoIngresado, MesIngresado + 1, 1);
            var ventas = await _ventaController.GetVentasAnuales(FechaIngresada.Year);
            VentasMensuales = new ObservableCollection<VentaMesDTO>(ventas);
        }

        private async Task CargarFinanzasMensuales()
        {
            // Ejemplo de implementación real
            DateTime FechaIngresada = new(AñoIngresado, MesIngresado + 1, 1);
            var ingresos = await _ingresoController.GetIngresosAnuales(FechaIngresada.Year);
            IngresosMensuales = new ObservableCollection<FinanzaMensualDTO>(ingresos);

            var egresos = await _egresoController.GetEgresosAnuales(FechaIngresada.Year);
            EgresosMensuales = new ObservableCollection<FinanzaMensualDTO>(egresos);
        }

        private async Task CargarStockPallets()
        {
            // Ejemplo de implementación real
            var stock = await _palletController.GetAllPallets();
            StockPallets = new ObservableCollection<Pallet>(stock);
        }

        private async Task CargarActivoPasivoAnuales()
        {
            // Ejemplo de implementación real
            DateTime FechaIngresada = new(AñoIngresado, MesIngresado + 1, 1);
            var activoPasivo = await _activoPasivoController.GetActivoPasivoAnual(FechaIngresada.Year);
            ActivoPasivoAnual = new ObservableCollection<ActivoPasivoMensualDTO>(activoPasivo);
        }

        private async Task CargarPedidosPendientes()
        {
            DateTime FechaIngresada = new(AñoIngresado, MesIngresado + 1, 1);
            var pedidos = await _pedidoController.GetPedidosPendientes(FechaIngresada);
            PedidosPendientes = new ObservableCollection<PedidoPendienteDTO>(pedidos);
        }

        private async Task CargarGastosFijos()
        {
            DateTime FechaIngresada = new(AñoIngresado, MesIngresado + 1, 1);
            var gastos = await _gastosFijosController.GetAllGastosFijos(FechaIngresada);
            GastosFijos = new ObservableCollection<GastosFijos>(gastos);

        }

        [RelayCommand]
        public async Task ActualizarDashboard()
        {
            await CargarDashboard();
        }

        [RelayCommand]
        public async Task NavegarAVentas()
        {
            await Shell.Current.GoToAsync("//VentasView");
        }

        [RelayCommand]
        public async Task NavegarAPedidos()
        {
            await Shell.Current.GoToAsync("//LotesView");
        }

        [RelayCommand]
        public async Task NavegarAFinanzas()
        {
            await Shell.Current.GoToAsync("//IngresosView");
        }


        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarDashboard();
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
                    await CargarDashboard();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
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