using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class PedidoModificarViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private readonly PedidoController _pedidoController;

        [ObservableProperty]
        public Pedido pedidoModificar;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public PedidoModificarViewModel(PedidoController pedidoController)
        {
            _pedidoController = pedidoController;
            PedidoModificar = new Pedido();
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Pedido", out var pedido) && pedido is Pedido _pedido)
            {
                PedidoModificar = _pedido;
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
        public async Task GuardarCambios()
        {
            if (PedidoModificar == null) return;

            if (ValidarPedido(PedidoModificar) == false)
            {
                await MostrarAlerta("Error", "Campos incompletos.");
                return;
            }

            try
            {
                MessageResult result = await _pedidoController.UpdatePedido(PedidoModificar);
                await MostrarAlerta(result.Title, result.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await VolverAtras();
        }

        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }

        }

        private bool ValidarPedido(Pedido pedido)
        {
            if (pedido.PalletId <= 0 || pedido.Cantidad <= 0 || pedido.LoteId <= 0)
                return false;
            return true;

        }
    }

}
