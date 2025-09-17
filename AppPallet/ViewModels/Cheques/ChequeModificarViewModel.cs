using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class ChequeModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        public Cheque? chequeSeleccionado;

        [ObservableProperty]
        public int estadoSeleccionado;
        public List<int> OpcionesEstado { get; } = new List<int> { 0, 1, 2, 3, 4 };

        public Dictionary<int, string> DescripcionesEstado { get; } = new Dictionary<int, string>
        {
            { 0, "Pendiente" },
            { 1, "En proceso" },
            { 2, "Pagado" },
            { 3, "Rechazado" },
            { 4, "Anulado" }
        };


        readonly IPopupService _popupService;

        readonly ChequeController _chequeController;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeModificarViewModel(IPopupService popupService, ChequeController chequeController)
        {
            _popupService = popupService;
            _chequeController = chequeController;
            EstadoSeleccionado = 0; 
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CancelarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (ChequeSeleccionado == null || !ValidarCheque())
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            ChequeSeleccionado.Estado = EstadoSeleccionado;

            try
            {
                var resultado = await _chequeController.UpdateCheque(ChequeSeleccionado);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Cheque actualizado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo actualizar el cheque");
                }
                ChequeSeleccionado = new Cheque();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CancelarPopup();
        }

        [RelayCommand]
        async Task EliminarCheque()
        {
            if (ChequeSeleccionado == null || ChequeSeleccionado.ChequeId == 0)
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            // Obtener la página principal de forma compatible con la nueva API
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }

            // Confirmar eliminación
            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar este cheque?", "Sí", "No");
            if (!confirmar)
                return;

            try
            {
                var resultado = await _chequeController.DeleteCheque(ChequeSeleccionado.ChequeId);
                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Cheque eliminado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el cheque");
                }
                ChequeSeleccionado = new Cheque();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }
            await CancelarPopup();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            ChequeSeleccionado = (Cheque)query["ChequeSeleccionado"];
            // Establecer el estado seleccionado basado en el estado del cheque
            if (ChequeSeleccionado != null && ChequeSeleccionado.Estado.HasValue)
            {
                EstadoSeleccionado = ChequeSeleccionado.Estado.Value;
            }
        }

        bool ValidarCheque()
        {
            if (ChequeSeleccionado == null) return false;
            if (ChequeSeleccionado.NumCheque == null || ChequeSeleccionado.NumCheque.Trim() == "") return false;
            if (ChequeSeleccionado.Proveedor == null || ChequeSeleccionado.Proveedor.Trim() == "") return false;
            if (ChequeSeleccionado.Tipo == null || ChequeSeleccionado.Tipo.Trim() == "") return false;
            if (ChequeSeleccionado.Monto <= 0) return false;
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
