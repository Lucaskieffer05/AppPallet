using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using static AppPallet.Constants.MessageConstants;

namespace AppPallet.ViewModels
{
    public partial class LoteViewModel : ObservableObject
    {


        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly LoteController _loteController;
        private readonly PedidoController _pedidoController;

        [ObservableProperty]
        public ObservableCollection<LoteMostrarDTO> listaLotes = [];

        [ObservableProperty]
        public bool isBusy;

        [ObservableProperty]
        public LoteMostrarDTO? loteSeleccionado;

        [ObservableProperty]
        public int totalPallets;

        [ObservableProperty]
        public int lotesPendientes;

        [ObservableProperty]
        public decimal inversionTotal;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;


        public ObservableCollection<string> Meses { get; } = new()
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

        public ObservableCollection<int> Años { get; } = new()
            {
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            };


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public LoteViewModel(LoteController loteController, PedidoController pedidoController)
        {
            _loteController = loteController;
            _pedidoController = pedidoController;
        }



        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaLotes()
        {

            try
            {
                IsBusy = true;

                var loteList = await _loteController.GetAllLotesMostrar(new DateTime(AñoIngresado, MesIngresado + 1, 1));
                ListaLotes = new ObservableCollection<LoteMostrarDTO>(loteList);

                TotalPallets = ListaLotes.Sum(l => l.TotalPallets);
                InversionTotal = ListaLotes.Sum(l => l.CostoTotal ?? 0);
                LotesPendientes = ListaLotes.Count(l => !l.FechaEntrega.HasValue);
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarListaLotes();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }

        partial void OnAñoIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await CargarListaLotes();
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }


        [RelayCommand]
        public async Task VerDetallesLote(LoteMostrarDTO lote)
        {
            LoteSeleccionado = lote;

            if (LoteSeleccionado == null)
            {
                await MostrarAlerta("Error", "No se ha seleccionado ninguna Lote.");
                return;
            }
            var navigationParams = new Dictionary<string, object>
            {
                {"LoteDTO", LoteSeleccionado}
            };
            await Shell.Current.GoToAsync(nameof(LoteModificarView), navigationParams);

        }

        [RelayCommand]
        public async Task VerCrearLote()
        {
            await Shell.Current.GoToAsync(nameof(LoteCrearView));
        }

        [RelayCommand]
        public async Task EliminarLote(LoteMostrarDTO lote)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null) return;

            bool confirmar = await mainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Estás seguro de eliminar el lote #{lote.NumLote}?",
                "Eliminar", "Cancelar");

            if (confirmar)
            {
                
                MessageResult resultado = await _loteController.DeleteLote(lote.LoteId);

                await MostrarAlerta(resultado.Title, resultado.Message);

                await CargarListaLotes();

            }
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
