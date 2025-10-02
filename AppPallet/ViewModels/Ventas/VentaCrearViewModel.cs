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

        [ObservableProperty]
        private Venta ventaCreated;

        [ObservableProperty]
        public bool presupuestoEnabled = false;

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
        private string estadoingresado;

        public ObservableCollection<string> Estados { get; } =
        [
            "En Producción", "Entregado"
        ];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaCrearViewModel(VentaController ventaController, EmpresaController empresaController, CostoPorPalletController costoPorPalletController, IngresoController ingresoController)
        {
            _ventaController = ventaController;
            _empresaController = empresaController;
            _costoPorPalletController = costoPorPalletController;
            _ingresoController = ingresoController;

            Estadoingresado = "En Producción";
            VentaCreated = new Venta
            {
                FechaVenta = DateTime.Now,
                CantPallets = 1,
                Estado = "En Producción",
                CostoPorPalletId = 0,
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
            VentaCreated.CostoPorPalletId = CostoPorPalletIngresado.CostoPorPalletId;
            VentaCreated.Estado = Estadoingresado;
            if(Estadoingresado == "Entregado" && VentaCreated.FechaEntrega == null)
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

            MessageResult resultVenta = await _ventaController.CreateVenta(VentaCreated);

            await MostrarAlerta(resultVenta.Title, resultVenta.Message);

            if (VentaCreated.Estado == "Entregado")
            {
                Ingreso ingreso = new Ingreso
                {
                    Fecha = VentaCreated.FechaEntrega,
                    DescripIngreso = $"Venta de {VentaCreated.CantPallets} pallets - {CostoPorPalletIngresado.NombrePalletCliente}",
                    Op = string.Empty,
                    Remito = string.Empty,
                    Factura = string.Empty,
                    Monto = (decimal)(VentaCreated.CantPallets * (CostoPorPalletIngresado.PrecioPallet ?? 0)),
                    Comentario = "ENTREGADO"
                };

                MessageResult resultIngreso = await _ingresoController.CreateIngreso(ingreso);

                await MostrarAlerta(resultIngreso.Title, resultIngreso.Message);
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
                PresupuestoEnabled = true;
            }
            else
            {
                CostoPorPalletIngresado = new CostoPorPalletDTO();
                VentaCreated.CostoPorPalletId = 0;
                PresupuestoEnabled = false;
            }
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
            if (venta.CantPallets <= 0)
                return false;
            if (venta.FechaVenta == default)
                return false;
            if (string.IsNullOrWhiteSpace(venta.Estado))
                return false;
            if (venta.CostoPorPalletId <= 0)
                return false;
            return true;

        }


    }
}
