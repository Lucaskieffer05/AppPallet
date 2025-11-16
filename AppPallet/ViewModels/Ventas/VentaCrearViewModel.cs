using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class VentaCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly VentaController _ventaController;

        readonly EmpresaController _empresaController;

        readonly CostoPorPalletController _costoPorPalletController;

        readonly IngresoController _ingresoController;

        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        private Venta ventaCreated;

        [ObservableProperty]
        public bool presupuestoEnabled = false;

        [ObservableProperty]
        public bool usarPrecioManual = false;

        [ObservableProperty]
        public decimal? precioManual;

        [ObservableProperty]
        public bool mostrarPresupuesto = true;

        [ObservableProperty]
        public bool entregadoFlag = false;

        [ObservableProperty]
        public ObservableCollection<Empresa> listEmpresas = [];

        [ObservableProperty]
        public Empresa? empresaIngresada;

        [ObservableProperty]
        public ObservableCollection<CostoPorPalletDTO> listCostoPorPallet = [];

        [ObservableProperty]
        public CostoPorPalletDTO costoPorPalletIngresado = new();

        [ObservableProperty]
        public int stockActual;


        [ObservableProperty]
        private string estadoingresado;

        public ObservableCollection<string> Estados { get; } =
        [
            "En Producción", "En Stock", "Entregado"
        ];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaCrearViewModel(VentaController ventaController, EmpresaController empresaController, CostoPorPalletController costoPorPalletController, IngresoController ingresoController, ActivoPasivoController activoPasivoController)
        {
            _ventaController = ventaController;
            _empresaController = empresaController;
            _costoPorPalletController = costoPorPalletController;
            _ingresoController = ingresoController;
            _activoPasivoController = activoPasivoController;

            Estadoingresado = "En Producción";
            VentaCreated = new Venta
            {
                FechaVenta = DateTime.Now,
                CantPallets = 0,
                Estado = "En Producción",
                CostoPorPalletId = null,
                Comentario = string.Empty,
                FechaEntrega = null
            };
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListas()
        {
            try
            {
                var listEmpresas = await _empresaController.GetAllAloneEmpresas();
                ListEmpresas = new ObservableCollection<Empresa>(listEmpresas);
                if (ListEmpresas.Count > 0)
                    EmpresaIngresada = ListEmpresas.FirstOrDefault()!;
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Hubo un error cargando las listas. Intente nuevamente. {ex}");
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
        async Task CrearVenta()
        {
            if (UsarPrecioManual)
            {
                VentaCreated.CostoPorPalletId = null;
                VentaCreated.PrecioManual = PrecioManual;
                VentaCreated.EmpresaId = EmpresaIngresada?.EmpresaId;
            }
            else
            {
                VentaCreated.CostoPorPalletId = CostoPorPalletIngresado?.CostoPorPalletId > 0 ? CostoPorPalletIngresado.CostoPorPalletId : null;
                VentaCreated.PrecioManual = null;
                // Opcional: asociar empresa también cuando hay presupuesto
                VentaCreated.EmpresaId = CostoPorPalletIngresado?.EmpresaId;
            }
            VentaCreated.Estado = Estadoingresado;
            if (Estadoingresado == "Entregado" && VentaCreated.FechaEntrega == null)
            {
                VentaCreated.FechaEntrega = DateTime.Now;
            }
            else if (Estadoingresado != "Entregado")
            {
                VentaCreated.FechaEntrega = null;
            }
            if (!ValidarVenta(VentaCreated))
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos obligatorios.");
                return;
            }

            if (Estadoingresado == "Entregado" && VentaCreated.FechaCobroEstimada == null)
            {
                await MostrarAlerta("Error", "No se puede crear la venta como 'Entregado' sin una fecha de cobro estimada.");
                return;
            }

            MessageResult resultVenta = await _ventaController.CreateVenta(VentaCreated);

            await MostrarAlerta(resultVenta.Title, resultVenta.Message);

            if (VentaCreated.Estado == "Entregado")
            {

                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }


                bool confirmarIngreso = await mainPage.DisplayAlert("Confirmar", "Se ha detectado el estado de 'Entregado' ¿Desea registar el ingreso de esta venta?", "Sí", "No");
                bool confirmarActivo = await mainPage.DisplayAlert("Confirmar", "¿Desea registrar también el activo de esta venta a partir de la fecha estimada de cobro?", "Sí", "No");

                var precioUnitario = (decimal)(CostoPorPalletIngresado?.PrecioPallet ?? VentaCreated.PrecioManual ?? 0);
                var empresaNombre = EmpresaIngresada?.NomEmpresa ?? "Sin empresa";
                var comentarioTexto = string.IsNullOrWhiteSpace(VentaCreated.Comentario) ? "Sin comentarios" : VentaCreated.Comentario;
                var montoTotal = VentaCreated.CostoPorPalletId != null
                    ? (decimal)(VentaCreated.CantPallets * precioUnitario)
                    : (decimal)(VentaCreated.PrecioManual ?? 0);

                if (confirmarIngreso)
                {
                    Ingreso ingreso = new Ingreso
                    {
                        Fecha = VentaCreated.FechaEntrega,
                        DescripIngreso = $"Venta a {empresaNombre} - {comentarioTexto}",
                        Op = string.Empty,
                        Remito = string.Empty,
                        Factura = string.Empty,
                        Monto = montoTotal,
                        Comentario = "ENTREGADO"
                    };
                    MessageResult resultIngreso = await _ingresoController.CreateIngreso(ingreso);
                    await MostrarAlerta(resultIngreso.Title, resultIngreso.Message);
                }

                if (confirmarActivo)
                {
                    ActivoPasivo activo = new ActivoPasivo
                    {
                        Fecha = VentaCreated.FechaCobroEstimada,
                        Mes = (DateTime)VentaCreated.FechaCobroEstimada,
                        Descripcion = $"Venta a {empresaNombre} - {comentarioTexto}",
                        Monto = montoTotal,
                        Categoria = "Activo",
                        Estado = "Sin Pagar"
                    };
                    MessageResult resultActivo = await _activoPasivoController.CreateActivoPasivo(activo);
                    await MostrarAlerta(resultActivo.Title, resultActivo.Message);
                }
            }

            await VolverAtras();

        }


        async partial void OnEmpresaIngresadaChanged(Empresa? value)
        {
            if (value == null)
            {
                return;
            }

            var presupuestos = await _costoPorPalletController.GetCostosPorPalletByEmpresaId(value.EmpresaId);
            ListCostoPorPallet = new ObservableCollection<CostoPorPalletDTO>(presupuestos);
            if (ListCostoPorPallet.Count > 0)
            {
                CostoPorPalletIngresado = ListCostoPorPallet.FirstOrDefault()!;
                PresupuestoEnabled = !UsarPrecioManual;
                MostrarPresupuesto = !UsarPrecioManual;
            }
            else
            {
                CostoPorPalletIngresado = new CostoPorPalletDTO();
                VentaCreated.CostoPorPalletId = null;
                PresupuestoEnabled = false;
                MostrarPresupuesto = false;
            }
        }

        partial void OnCostoPorPalletIngresadoChanged(CostoPorPalletDTO value)
        {
            if (value == null)
            {
                return;
            }

            if (value.Pallet != null)
                StockActual = value.Pallet.Stock;
        }

        partial void OnEstadoingresadoChanged(string value)
        {
            if (VentaCreated == null)
            {
                return;
            }

            if (value == "Entregado")
            {
                EntregadoFlag = true;
                VentaCreated.FechaEntrega = DateTime.Now;
            }
            else
            {
                VentaCreated.FechaEntrega = DateTime.MinValue;
                EntregadoFlag = false;
            }
        }

        partial void OnUsarPrecioManualChanged(bool value)
        {
            MostrarPresupuesto = !value;
            PresupuestoEnabled = !value && ListCostoPorPallet.Count > 0;
        }

        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }

        }


        private bool ValidarVenta(Venta venta)
        {
            if (venta.CantPallets < 0)
                return false;
            if (venta.FechaVenta == default)
                return false;
            if (string.IsNullOrWhiteSpace(venta.Estado))
                return false;
            if (UsarPrecioManual)
            {
                if (PrecioManual == null || PrecioManual <= 0)
                    return false;
            }
            else
            {
                if (venta.CostoPorPalletId == null || venta.CostoPorPalletId <= 0)
                    return false;
            }
            return true;

        }


    }
}
