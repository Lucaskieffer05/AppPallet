using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace AppPallet.ViewModels
{
    public partial class ChequeCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        [ObservableProperty]
        public Cheque? chequeCreated;

        readonly IPopupService _popupService;

        readonly ChequeController _chequeController;


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

        public bool IsBusy { get; private set; }

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeCrearViewModel(IPopupService popupService, ChequeController chequeController)
        {
            _popupService = popupService;
            _chequeController = chequeController;
            ChequeCreated = new Cheque();
            ChequeCreated.FechaEmision = DateTime.Today;
            ChequeCreated.FechaPago = DateTime.Today.AddDays(1);
            EstadoSeleccionado = 0;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CerrarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (ChequeCreated == null || !ValidarCheque(ChequeCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            ChequeCreated.Estado = EstadoSeleccionado;

            try
            {
                var resultado = await _chequeController.CreateCheque(ChequeCreated);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Cheque creado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo crear el cheque");
                }
                ChequeCreated = new Cheque();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CerrarPopup();
        }


        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }

        bool ValidarCheque(Cheque cheque)
        {
            if (cheque.Proveedor == null || cheque.Proveedor.Trim() == "") return false;
            if (cheque.Tipo == null || cheque.Tipo.Trim() == "") return false;
            if (cheque.Monto <= 0) return false;
            return true;
        }

    }
}
