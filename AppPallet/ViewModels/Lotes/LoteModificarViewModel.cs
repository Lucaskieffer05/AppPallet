using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class LoteModificarViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private readonly LoteController _loteController;
        private readonly PedidoController _pedidoController;
        private readonly PalletController _palletController;

        [ObservableProperty]
        public LoteMostrarDTO? loteModified;

        [ObservableProperty]
        public DateTime? fechaEntrega;

        [ObservableProperty]
        public string? estado;

        private string? CondicionInicialEstado;

        [ObservableProperty]
        public string? estadoColor;

        [ObservableProperty]
        public ObservableCollection<PedidoMostrarDTO> pedidos = [];

        [ObservableProperty]
        public int cantidadPallets;

        [ObservableProperty]
        public int cantidadPedidos;

        [ObservableProperty]
        public ObservableCollection<Pallet> palletsDisponibles = [];

        [ObservableProperty]
        public bool isBusy;

        [ObservableProperty]
        public PedidoMostrarDTO? pedidoSeleccionado;



        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public LoteModificarViewModel(LoteController loteController, PedidoController pedidoController, PalletController palletController)
        {
            _loteController = loteController;
            _pedidoController = pedidoController;
            _palletController = palletController;
        }
        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("LoteDTO", out var loteDTO) && loteDTO is LoteMostrarDTO _loteDTO)
            {
                LoteModified = _loteDTO;
                CondicionInicialEstado = LoteModified.Estado;
                FechaEntrega = LoteModified.FechaEntrega;
                Estado = FechaEntrega.HasValue ? "Entregado" : "Pendiente";
                EstadoColor = FechaEntrega.HasValue ? "#4CAF50" : "#FF9800";
                Task.Run(async () => await CargarDatosLote());
            }
        }

        partial void OnFechaEntregaChanged(DateTime? value)
        {
            // Solución para CS0019 y CS8629:
            // - value es de tipo DateTime? (acepta nulos)
            // - value.Value solo es válido si value.HasValue es true
            // - Asignar directamente value (que puede ser null) a FechaEntrega

            LoteModified!.FechaEntrega = value;
            Estado = FechaEntrega.HasValue ? "Entregado" : "Pendiente";
            EstadoColor = FechaEntrega.HasValue ? "#4CAF50" : "#FF9800";

        }

        [RelayCommand]
        public async Task CargarDatosLote()
        {
            if (LoteModified == null) return;

            try
            {
                IsBusy = true;

                // Cargar pedidos del lote
                var pedidos = await _pedidoController.GetPedidosByLoteId(LoteModified.LoteId);
                Pedidos = new ObservableCollection<PedidoMostrarDTO>(
                    pedidos.Select(p => new PedidoMostrarDTO(p))
                );

                CantidadPallets = Pedidos.Sum(p => p.Cantidad);
                CantidadPedidos = Pedidos.Count;

                // Cargar pallets disponibles
                var pallets = await _palletController.GetAllPallets();
                PalletsDisponibles = new ObservableCollection<Pallet>(pallets);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GuardarLote()
        {
            if (LoteModified == null) return;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(LoteModified.NomProveedor) ||
                string.IsNullOrWhiteSpace(LoteModified.NomCamionero))
            {
                await MostrarAlerta("Error", "Complete todos los campos obligatorios");
                return;
            }

            // Ver si cambio el estado y actuar en consecuencia
            if (CondicionInicialEstado != Estado && Estado== "Entregado")
            {
                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage == null) return;

                var confirmar = await mainPage.DisplayAlert(
                    "Confirmar entrega",
                    "Se ha detectado la entrega del lote. El stock correspondiente será sumado a los pedidos registrados. ¿Desea continuar con esta acción?",
                    "Confirmar", "Cancelar");

                if (confirmar)
                {
                    // sumar stock
                    // recorrer los pedidos y sumar stock a cada pallet
                    foreach (var pedido in Pedidos)
                    {
                        MessageResult result = await _palletController.SumarStockPallet(pedido.PalletId, pedido.Cantidad);
                        if (result.Title == MessageConstants.Titles.Error)
                        {
                            await MostrarAlerta(result.Title, result.Message);
                        }
                    }
                    await MostrarAlerta("Éxito", "El stock ha sido actualizado correctamente.");
                }

            }
            LoteModified.FechaEntrega = FechaEntrega;

            MessageResult resultado = await _loteController.UpdateLote(LoteModified);

            await MostrarAlerta(resultado.Title, resultado.Message);

            await VolverAtras();
        }

        [RelayCommand]
        public async Task AgregarPedido()
        {
            if (LoteModified == null) return;

            var navigationParameters = new Dictionary<string, object>
            {
                { "LoteId", LoteModified.LoteId }
            };
            await Shell.Current.GoToAsync(nameof(PedidoCrearView), navigationParameters);
        }

        [RelayCommand]
        public async Task ModificarPedido(PedidoMostrarDTO pedido)
        {
            if (pedido == null) return;

            var navigationParameters = new Dictionary<string, object>
            {
                { "Pedido", pedido }
            };
            await Shell.Current.GoToAsync(nameof(PedidoModificarView), navigationParameters);
        }

        [RelayCommand]
        public async Task EliminarPedido(PedidoMostrarDTO pedido)
        {
            if (pedido == null) return;

            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null) return;

            var confirmar = await mainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Estás seguro de eliminar este pedido?",
                "Eliminar", "Cancelar");

            if (confirmar)
            {

                if (Estado == "Entregado")
                {

                    var confirmarStock = await mainPage.DisplayAlert(
                        "Confirmar entrega",
                        "Este lote ya fue entregado. ¿Desea descontar del stock la cantidad solicitada?",
                        "Sí, descontar", "No descontar");

                    if (confirmarStock)
                    {
                        MessageResult result = await _palletController.SumarStockPallet(pedido.PalletId, -pedido.Cantidad);
                        if (result.Title == MessageConstants.Titles.Error)
                        {
                            await MostrarAlerta(result.Title, result.Message);
                            return;
                        }
                        await MostrarAlerta("Éxito", "El stock ha sido actualizado correctamente.");
                    }

                }
                var resultado = await _pedidoController.DeletePedido(pedido.PedidoId);
                await MostrarAlerta(resultado.Title, resultado.Message);
                await CargarDatosLote();
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


        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
        }


    }
}
