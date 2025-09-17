using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppPallet.ViewModels
{
    public partial class ChequeViewModel : ObservableObject, INotifyPropertyChanged
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly ChequeController _chequeController;

        private readonly IPopupService _popupService;

        [ObservableProperty]
        private ObservableCollection<Cheque> listaCheques = [];

        [ObservableProperty]
        public Cheque? chequeSeleccionado;

        [ObservableProperty]
        private bool isBusy;

        private bool _isLoading = false;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeViewModel(IPopupService popupService, ChequeController chequeController)

        {
            _popupService = popupService;
            _chequeController = chequeController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaCheques()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                IsBusy = true;

                ChequeSeleccionado = null;

                var chequesList = await _chequeController.GetAllCheques();
                ListaCheques = new ObservableCollection<Cheque>(chequesList);
            }
            finally
            {
                IsBusy = false;
                _isLoading = false;
            }
        }


        [RelayCommand]
        public async Task MostrarPopupCheque()
        {
            
            await DisplayPopupCrear();
        }

        [RelayCommand]
        public async Task MostrarPopupChequeModificar()
        {
            if (ChequeSeleccionado == null)
            {
                await MostrarAlerta("Atención", "Debe seleccionar un cheque para modificar.");
            }
            else
            {
                await DisplayPopupModificar();
            }
        }

        public async Task DisplayPopupCrear()
        {
            var popupResult = await _popupService.ShowPopupAsync<ChequeCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty);
        }

        public async Task DisplayPopupModificar()
        {
            if (ChequeSeleccionado == null)
                throw new InvalidOperationException("ChequeSeleccionado no puede ser nulo al modificar.");

            var queryAttributes = new Dictionary<string, object>
            {
                ["ChequeSeleccionado"] = ChequeSeleccionado
            };

            var popupResult = await _popupService.ShowPopupAsync<ChequeModificarViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);

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
