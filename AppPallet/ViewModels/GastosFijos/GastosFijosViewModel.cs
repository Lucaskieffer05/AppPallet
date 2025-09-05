using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class GastosFijosViewModel : ObservableObject, INotifyPropertyChanged
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly IPopupService _popupService;

        readonly GastosFijosController _gastosFijosController;

        [ObservableProperty]
        public ObservableCollection<GastosFijo> listaGastosFijos = [];

        [ObservableProperty]
        public GastosFijo? gastoFijoSeleccionado;

        [ObservableProperty]
        public bool isBusy;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public GastosFijosViewModel(IPopupService popupService, GastosFijosController gastosFijosController)
        {
            _popupService = popupService;
            _gastosFijosController = gastosFijosController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaGastosFijos()
        {

            try
            {
                IsBusy = true;

                GastoFijoSeleccionado = null;

                var gastosFijoList = await _gastosFijosController.GetAllGastosFijos();
                ListaGastosFijos = new ObservableCollection<GastosFijo>(gastosFijoList);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task MostrarPopupGastoFijo()
        {
            await DisplayPopupCrear();
        }

        [RelayCommand]
        public async Task MostrarPopupModificarGastoFijo()
        {
            if (GastoFijoSeleccionado == null)
            {
                await MostrarAlerta("Error", "Seleccione un gasto fijo para modificar.");
                return;
            }
            await DisplayPopupModificar();
        }

        public async Task DisplayPopupCrear()
        {
            var popupResult = await _popupService.ShowPopupAsync<GastosFijosCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty);

        }

        public async Task DisplayPopupModificar()
        {
            if (GastoFijoSeleccionado == null)
                throw new InvalidOperationException("ChequeSeleccionado no puede ser nulo al modificar.");

            var queryAttributes = new Dictionary<string, object>
            {
                ["GastoFijoSeleccionado"] = GastoFijoSeleccionado
            };

            var popupResult = await _popupService.ShowPopupAsync<GastosFijosModificarViewModel>(
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
