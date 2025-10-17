using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class LoteCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        readonly LoteController _loteController;
        readonly EmpresaController _empresaController;

        [ObservableProperty]
        public Lote loteCreated;

        [ObservableProperty]
        private ObservableCollection<Empresa> listaProveedores = [];

        [ObservableProperty]
        public Empresa? proveedorSeleccionado;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public LoteCrearViewModel(LoteController loteController, EmpresaController empresaController)
        {
            _loteController = loteController;
            _empresaController = empresaController;
            LoteCreated = new();

            LoteCreated.FechaSolicitada = DateTime.Today;

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarProveedores()
        {

            try
            {
                var proveedores = await _empresaController.GetAllEmpresas("Proveedor");
                ListaProveedores = new ObservableCollection<Empresa>(proveedores);

            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error al cargar los proveedores: {ex.Message}");
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
        async Task CrearLote()
        {
            if (LoteCreated == null || !ValidarLote(LoteCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {
                LoteCreated.EmpresaId = ProveedorSeleccionado!.EmpresaId;
                var resultado = await _loteController.CreateLote(LoteCreated);
                await MostrarAlerta(resultado.Title, resultado.Message);

                if (resultado.Title == MessageConstants.Titles.Success)
                {
                    await VolverAtras();
                }
                return;

            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
                return;

            }

        }


        private bool ValidarLote(Lote lote)
        {
            if (ProveedorSeleccionado == null) return false;
            if (lote.FechaSolicitada == default) return false;
            if (string.IsNullOrWhiteSpace(lote.NomCamionero)) return false;
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
