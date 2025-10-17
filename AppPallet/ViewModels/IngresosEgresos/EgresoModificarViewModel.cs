using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class EgresoModificarViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private EgresoController _egresoController;

        [ObservableProperty]
        public Egreso? egresoModificar;

        [ObservableProperty]
        private bool conIva;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public EgresoModificarViewModel(EgresoController egresoController)
        {
            _egresoController = egresoController;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Egreso", out var egreso) && egreso is Egreso _egreso)
            {
                EgresoModificar = _egreso;
                ConIva = EgresoModificar.SumaIva.HasValue && EgresoModificar.SumaIva.Value > 0;
            }
        }

        [RelayCommand]
        async Task VolverAtras()
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
        async Task ModificarEgreso()
        {
            if (EgresoModificar == null || !ValidarEgreso(EgresoModificar))
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos correctamente.");
                return;
            }
            try
            {
                double iva = Preferences.Get("iva_percentage", 0.21);
                EgresoModificar.SumaIva = ConIva ? EgresoModificar.Monto * (decimal)iva : null;
                bool exito = await _egresoController.UpdateEgreso(EgresoModificar);
                if (exito)
                {
                    await MostrarAlerta("Éxito", "Egreso modificado correctamente.");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo modificar el egreso. Intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }

            await VolverAtras();

        }

        [RelayCommand]
        void ToggleIva()
        {
            ConIva = !ConIva;
        }

        [RelayCommand]
        async Task EliminarEgreso()
        {
            if (EgresoModificar == null)
            {
                await MostrarAlerta("Error", "No hay egreso seleccionado para eliminar.");
                return;
            }

            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }

            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar este egreso?", "Sí", "No");

            if (!confirmar) return;
            try
            {
                bool exito = await _egresoController.DeleteEgreso(EgresoModificar.EgresoId);
                if (exito)
                {
                    await MostrarAlerta("Éxito", "Egreso eliminado correctamente.");
                    await VolverAtras();
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el egreso. Intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
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

        private bool ValidarEgreso(Egreso egreso)
        {
            if (egreso == null) return false;
            if (string.IsNullOrWhiteSpace(egreso.DescripEgreso)) return false;
            if (egreso.Monto <= 0) return false;
            if (egreso.Fecha == default) return false;
            return true;

        }
    }
}
