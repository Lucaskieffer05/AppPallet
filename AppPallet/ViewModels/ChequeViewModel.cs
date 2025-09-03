using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.ViewModels;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModel
{
    public partial class ChequeViewModel : ObservableObject
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

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeViewModel(IPopupService popupService)

        {
            _popupService = popupService;
            _chequeController = new ChequeController(new PalletContex());
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task GetAllCheques()
        {
            IsBusy = true;
            var chequesList = await _chequeController.GetAllCheques();
            ListaCheques = new ObservableCollection<Cheque>(chequesList);
            IsBusy = false;
        }


        [RelayCommand]
        public async Task MostrarPopupCheque()
        {
            await DisplayPopup();
        }

        [RelayCommand]
        public async Task MostrarPopupChequeModificar()
        {
            await DisplayPopupModificar();
        }

        public async Task DisplayPopup()
        {
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(ChequeCrearViewModel.Name)] = "Shaun"
            };

            await _popupService.ShowPopupAsync<ChequeCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);
        }

        public async Task DisplayPopupModificar()
        {
            var queryAttributes = new Dictionary<string, object>
            {
                ["ChequeSeleccionado"] = ChequeSeleccionado
            };

            await _popupService.ShowPopupAsync<ChequeModificarViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);
        }

    }
}
