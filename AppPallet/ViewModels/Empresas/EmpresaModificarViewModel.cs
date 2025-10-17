using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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

        public ObservableCollection<string> Tipos { get; } =
            [
                "Cliente", "Proveedor"
            ];



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
        async Task Eliminar()
        {
            try
            {
                if (EmpresaSeleccionada == null)
                {
                    await MostrarAlerta("Error", "No hay empresa seleccionada");
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
                bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar esta empresa?", "Sí", "No");
                if (!confirmar)
                    return;

                MessageResult resultado = await _empresaController.DeleteEmpresa(EmpresaSeleccionada.EmpresaId);
                
                await MostrarAlerta(resultado.Title, resultado.Message);

                await CancelarPopup();

            }
            catch(Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }
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
            if (string.IsNullOrWhiteSpace(EmpresaSeleccionada?.NomEmpresa))
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
