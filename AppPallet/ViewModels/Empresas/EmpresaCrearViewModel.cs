using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class EmpresaCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------


        [ObservableProperty]
        public Empresa? empresaCreated;

        readonly IPopupService _popupService;

        readonly EmpresaController _empresaController;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public EmpresaCrearViewModel(IPopupService popupService, EmpresaController empresaController)
        {
            _popupService = popupService;
            _empresaController = empresaController;
            EmpresaCreated = new Empresa();
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
            if (EmpresaCreated == null || !ValidarEmpresa(EmpresaCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                var resultado = await _empresaController.CreateEmpresa(EmpresaCreated);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Empresa creada correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo crear la empresa");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
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

        private bool ValidarEmpresa(Empresa empresa)
        {
            if (string.IsNullOrWhiteSpace(empresa.NomEmpresa))
                return false;
            if (string.IsNullOrWhiteSpace(empresa.Cuit))
                return false;
            if (string.IsNullOrWhiteSpace(empresa.Domicilio))
                return false;
            return true;
        }


    }
}
