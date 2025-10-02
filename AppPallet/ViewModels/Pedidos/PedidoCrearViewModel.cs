using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class PedidoCrearViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private readonly PedidoController _pedidoController;
        private readonly PalletController _palletController;
        private readonly LoteController _loteController;

        [ObservableProperty]
        public Pedido pedidoCreated = new();

        [ObservableProperty]
        public DateTime? fechaEInicio;

        [ObservableProperty]
        public DateTime? fechaEFinal;

        [ObservableProperty]
        public bool hasFechaFinal = false;

        [ObservableProperty]
        public int cantidad;

        [ObservableProperty]
        public ObservableCollection<Pallet> palletsDisponibles = [];

        [ObservableProperty]
        public Pallet? palletSeleccionado;

        [ObservableProperty]
        public Lote? loteInfo;

        [ObservableProperty]
        public bool isBusy;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public PedidoCrearViewModel(PedidoController pedidoController, PalletController palletController, LoteController loteController)
        {
            _pedidoController = pedidoController;
            _palletController = palletController;
            _loteController = loteController;
        }



        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("LoteId", out var loteId) && loteId is int _loteId)
            {
                PedidoCreated.LoteId = _loteId;
                await CargarDatosIniciales(_loteId);
            }
        }

        partial void OnFechaEFinalChanged(DateTime? value)
        {
            if (value != null)
            {
                HasFechaFinal = true;
            }
            else
            {
                HasFechaFinal = false;
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
        public async Task CargarDatosIniciales(int loteId)
        {
            try
            {
                IsBusy = true;

                // Cargar información del lote
                LoteInfo = await _loteController.GetLoteById(loteId);

                // Cargar pallets disponibles
                var pallets = await _palletController.GetAllPallets();
                PalletsDisponibles = new ObservableCollection<Pallet>(pallets);

                // Establecer fecha de inicio por defecto
                PedidoCreated.FechaEInicio = DateTime.Today;
            }
            finally
            {
                IsBusy = false;
            }
        }



        [RelayCommand]
        public async Task CrearPedido()
        {
            if (!ValidarPedido())
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos obligatorios correctamente.");
                return;
            }

            try
            {
                IsBusy = true;

                // Asignar el PalletId seleccionado
                if (PalletSeleccionado != null)
                {
                    PedidoCreated.PalletId = PalletSeleccionado.PalletId;
                }

                PedidoCreated.FechaEInicio = FechaEInicio;
                PedidoCreated.FechaEFinal = FechaEFinal;
                PedidoCreated.Cantidad = Cantidad;



                if (LoteInfo != null && LoteInfo.FechaEntrega != null)
                {
                    var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (mainPage == null) return;

                    var confirmarStock = await mainPage.DisplayAlert(
                        "Confirmar entrega",
                        $"Este lote ya fue entregado. ¿Desea suamar la cantidad solicitada de {Cantidad} al stock del pallet {PalletSeleccionado!.Nombre}?",
                        "Sí, sumar", "No sumar");

                    if (confirmarStock)
                    {
                        MessageResult result = await _palletController.SumarStockPallet(PalletSeleccionado.PalletId, Cantidad);
                        if (result.Title == MessageConstants.Titles.Error)
                        {
                            await MostrarAlerta(result.Title, result.Message);
                            return;
                        }
                        await MostrarAlerta("Éxito", "El stock ha sido actualizado correctamente.");
                    }

                }
                MessageResult resultado = await _pedidoController.CreatePedido(PedidoCreated);

                await MostrarAlerta(resultado.Title, resultado.Message);

                if (resultado.Title == MessageConstants.Titles.Success)
                {
                    await VolverAtras();
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error al crear el pedido: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidarPedido()
        {
            if (PedidoCreated == null)
                return false;

            if (PalletSeleccionado == null)
                return false;

            if (Cantidad <= 0)
                return false;

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



    }
}
