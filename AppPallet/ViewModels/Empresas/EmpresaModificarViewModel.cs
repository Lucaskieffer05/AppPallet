using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class EmpresaModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        public Empresa? empresaSeleccionada;

        readonly IPopupService _popupService;

        readonly EmpresaController _empresaController;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public EmpresaModificarViewModel(IPopupService popupService, EmpresaController empresaController)
        {
            _popupService = popupService;
            _empresaController = empresaController;
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
            if (EmpresaSeleccionada == null || !ValidarEmpresa())
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                var resultado = await _empresaController.UpdateEmpresa(EmpresaSeleccionada);
                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Empresa actualizada correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo actualizar la empresa");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                await _popupService.ClosePopupAsync(Shell.Current);
            }
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            EmpresaSeleccionada = (Empresa)query["EmpresaSeleccionada"];
        }

        bool ValidarEmpresa()
        {
            if (string.IsNullOrWhiteSpace(EmpresaSeleccionada?.NomEmpresa) ||
                string.IsNullOrWhiteSpace(EmpresaSeleccionada?.Domicilio) ||
                string.IsNullOrWhiteSpace(EmpresaSeleccionada?.Cuit))
            {
                return false;
            }
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
