using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class EmpresaViewModel : ObservableObject, INotifyPropertyChanged
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        private bool isBusy;

        readonly EmpresaController _EmpresaController;

        private readonly IPopupService _popupService;

        [ObservableProperty]
        private ObservableCollection<Empresa> listaEmpresas = [];

        [ObservableProperty]
        public Empresa? empresaSeleccionada;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public EmpresaViewModel(IPopupService popupService, EmpresaController chequeController)
        {
            _popupService = popupService;
            _EmpresaController = chequeController;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaClientesProveedores()
        {
            try
            {
                IsBusy = true;
                var lista = await _EmpresaController.GetAllClientesProveedores();
                ListaClientesProveedores = new ObservableCollection<Empresa>(lista);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task MostrarPopupCrear()
        {
            await DisplayPopupCrear();
        }

        [RelayCommand]
        public async Task MostrarPopupModificar()
        {
            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Atención", "Debe seleccionar un cheque para modificar.");
                return;
            }
            await DisplayPopupModificar();
        }

        public async Task DisplayPopupCrear()
        {
            var popupResult = await _popupService.ShowPopupAsync<EmpresaCrearViewModel>(
                Shell.Current,
                options: PopupOptions.Empty);
        }

        public async Task DisplayPopupModificar()
        {
            var parameters = new Dictionary<string, object>
            {
                { "Empresa", EmpresaSeleccionada! }
            };
            var popupResult = await _popupService.ShowPopupAsync<EmpresaModificarViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                parameters);
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
