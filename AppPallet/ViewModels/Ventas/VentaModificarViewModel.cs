using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class VentaModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly VentaController _ventaController;

        readonly IngresoController _ingresoController;

        readonly ActivoPasivoController _activoPasivoController;

        readonly PalletController _palletController;

        [ObservableProperty]
        public Venta? ventaModified;

        [ObservableProperty]
        public bool enStock;

        [ObservableProperty]
        public string tituloPage = string.Empty;

        private string auxEstado = string.Empty;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public VentaModificarViewModel(VentaController ventaController, IngresoController ingresoController, ActivoPasivoController activoPasivoController, PalletController palletController)
        {
            _ventaController = ventaController;
            _ingresoController = ingresoController;
            _activoPasivoController = activoPasivoController;
            _palletController = palletController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("VentaId", out var ventaId) && ventaId is int _ventaId)
            {
                _ = CargarVentaAsync(_ventaId);

            }
        }

        private async Task CargarVentaAsync(int ventaId)
        {
            VentaModified = await _ventaController.GetVentaById(ventaId);


            if (VentaModified == null)
            {
                await MostrarAlerta("Venta Cargada", $"Venta ID: {ventaId} no fue cargada correctamente.");
            }
            else
            {
                TituloPage = $"Modificar venta a {VentaModified.CostoPorPallet.Empresa.NomEmpresa} con precio: ${VentaModified.CostoPorPallet.PrecioPallet}";
                auxEstado = VentaModified.Estado;
                EnStock = VentaModified.Estado == "En Stock";
            }
        }


        [RelayCommand]
        public async Task VolverAtras()
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
        public async Task ModificarVenta()
        {
            if (VentaModified != null && ValidarVenta(VentaModified))
            {
                if(VentaModified.FechaEntrega != null)
                {
                    VentaModified.Estado = "Entregado";
                }
                else if (EnStock)
                {
                    VentaModified.Estado = "En Stock";
                }
                else
                {
                    VentaModified.Estado = "En Producción";
                }

                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }

                if (VentaModified.Estado != auxEstado && VentaModified.Estado == "Entregado")
                {
                    if (VentaModified.FechaCobroEstimada == null)
                    {
                        // Mostrar alerta y salir
                        await MostrarAlerta("Error", "No se puede cambiar el estado a 'Entregado' sin una fecha de cobro estimada.");
                        return;
                    }

                    bool confirmarIngreso = await mainPage.DisplayAlert("Confirmar", "Se ha detectado el estado de 'Entregado' ¿Desea registar el ingreso de esta venta?", "Sí", "No");
                    bool confirmarActivo = await mainPage.DisplayAlert("Confirmar", "¿Desea registrar también el activo de esta venta a partir de la fecha estimada de cobro?", "Sí", "No");

                    if (confirmarIngreso)
                    {
                        Ingreso ingreso = new Ingreso
                        {
                            Fecha = VentaModified.FechaEntrega,
                            DescripIngreso = $"Venta de {VentaModified.CantPallets} pallets - {VentaModified.CostoPorPallet.NombrePalletCliente}",
                            Op = string.Empty,
                            Remito = string.Empty,
                            Factura = string.Empty,
                            Monto = (decimal)(VentaModified.CantPallets * (VentaModified.CostoPorPallet.PrecioPallet ?? 0)),
                            Comentario = "ENTREGADO"
                        };


                        MessageResult resultIngreso = await _ingresoController.CreateIngreso(ingreso);

                        await MostrarAlerta(resultIngreso.Title, resultIngreso.Message);
                    }

                    //registrar activo
                    if (confirmarActivo)
                    {
                        ActivoPasivo activo = new ActivoPasivo
                        {
                            Fecha = VentaModified.FechaCobroEstimada,
                            Mes = (DateTime)VentaModified.FechaCobroEstimada,
                            Descripcion = $"Venta a {VentaModified.CostoPorPallet.Empresa.NomEmpresa} por {VentaModified.CantPallets} pallets",
                            Monto = (decimal)(VentaModified.CantPallets * (VentaModified.CostoPorPallet.PrecioPallet ?? 0)),
                            Categoria = "Activo",
                            Estado = "Sin Pagar"
                        };
                        MessageResult resultActivo = await _activoPasivoController.CreateActivoPasivo(activo);
                        await MostrarAlerta(resultActivo.Title, resultActivo.Message);
                    }

                    // Restar stock
                    MessageResult palletResult = await _palletController.SumarStockPallet(VentaModified.CostoPorPallet.PalletId, -VentaModified.CantPallets);
                    
                    await MostrarAlerta(palletResult.Title, palletResult.Message);

                }


                MessageResult result = await _ventaController.UpdateVenta(VentaModified);

                await MostrarAlerta(result.Title, result.Message);
                await VolverAtras();
            }
            else
            {
                await MostrarAlerta("Error", "No hay datos de venta para modificar.");
            }
        }

        [RelayCommand]
        public async Task EliminarVenta()
        {
            if (VentaModified != null)
            {
                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null)
                {
                    await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                    return;
                }

                bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar esta Venta?", "Sí", "No");

                if (confirmar)
                {
                    MessageResult result = await _ventaController.DeleteVenta(VentaModified.VentaId);
                    await MostrarAlerta(result.Title, result.Message);
                    await VolverAtras();
                }
            }
            else
            {
                await MostrarAlerta("Error", "No hay datos de venta para eliminar.");
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
