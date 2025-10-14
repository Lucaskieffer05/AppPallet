using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppPallet.ViewModels
{
    public partial class HistorialHumedadViewModel : ObservableObject, IQueryAttributable
    {
        private readonly HistorialHumedadController _historialController;
        private readonly PedidoController _pedidoController;

        [ObservableProperty]
        private ObservableCollection<HistorialHumedadExtended> historialHumedad = [];

        [ObservableProperty]
        private PedidoMostrarDTO? pedidoInfo;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private HistorialHumedadExtended? nuevoRegistro;

        public HistorialHumedadViewModel(HistorialHumedadController historialController, PedidoController pedidoController)
        {
            _historialController = historialController;
            _pedidoController = pedidoController;
            InicializarNuevoRegistro();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Pedido", out var pedido) && pedido is PedidoMostrarDTO _pedido)
            {
                PedidoInfo = _pedido;
                await CargarHistorialHumedad(_pedido.PedidoId);
            }
        }

        [RelayCommand]
        public async Task CargarHistorialHumedad(int pedidoId)
        {
            try
            {
                IsBusy = true;
                var historial = await _historialController.GetHistorialByPedidoId(pedidoId);

                HistorialHumedad = new ObservableCollection<HistorialHumedadExtended>(
                    historial.Select(h => new HistorialHumedadExtended(h))
                );

                // Inicializar nuevo registro con el pedidoId
                if (NuevoRegistro != null)
                {
                    NuevoRegistro.PedidoId = pedidoId;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AgregarRegistroHumedad()
        {
            if (NuevoRegistro == null || !ValidarRegistro(NuevoRegistro))
            {
                await MostrarAlerta("Error", "Complete todos los campos correctamente.");
                return;
            }

            try
            {
                IsBusy = true;
                NuevoRegistro.Promedio = (NuevoRegistro.Min + NuevoRegistro.Max) / 2;
                MessageResult resultado = await _historialController.CreateHistorialHumedad(NuevoRegistro);

                await MostrarAlerta(resultado.Title, resultado.Message);
            }
            finally
            {
                IsBusy = false;
                InicializarNuevoRegistro();
                if (PedidoInfo != null)
                {
                    await CargarHistorialHumedad(PedidoInfo.PedidoId);
                }
            }
        }

        [RelayCommand]
        public async Task EliminarRegistro(HistorialHumedadExtended registro)
        {
            if (registro == null) return;

            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null) return;

            var confirmar = await mainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Estás seguro de eliminar el registro del {registro.Fecha:dd/MM/yyyy}?",
                "Eliminar", "Cancelar");

            if (confirmar)
            {
                MessageResult resultado = await _historialController.DeleteHistorialHumedad(registro.HistorialHumedadId);
                await MostrarAlerta(resultado.Title, resultado.Message);
                if (PedidoInfo != null)
                {
                    await CargarHistorialHumedad(PedidoInfo.PedidoId);
                }
            }
        }

        [RelayCommand]
        public async Task VolverAtras()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void InicializarNuevoRegistro()
        {
            NuevoRegistro = new HistorialHumedadExtended
            {
                Fecha = DateTime.Today,
                Min = 0,
                Max = 0,
                Promedio = 0
            };
        }

        private bool ValidarRegistro(HistorialHumedadExtended registro)
        {
            if (registro.Min <= 0 || registro.Max <= 0)
                return false;

            if (registro.Max < registro.Min)
                return false;

            if (registro.PedidoId <= 0)
                return false;

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

    public class HistorialHumedadExtended : HistorialHumedad
    {
        private int humedad_seco = Preferences.Get("humedad_seco", 12);
        private int humedad_optimo = Preferences.Get("humedad_optimo", 18);
        private int humedad_humedo = Preferences.Get("humedad_humedo", 22);
        public string EstadoHumedad
        {
            get
            {
                if (Promedio < humedad_seco) return "Muy Seco";
                if (Promedio <= humedad_optimo) return "Óptimo";
                if (Promedio <= humedad_humedo) return "Húmedo";
                return "Muy Húmedo";
            }
        }

        public string EstadoColor
        {
            get
            {
                if (Promedio < humedad_seco) return "#EF4444"; // Rojo - Muy Seco
                if (Promedio <= humedad_optimo) return "#10B981"; // Verde - Óptimo
                if (Promedio <= humedad_humedo) return "#F59E0B"; // Amarillo - Húmedo
                return "#5163D6"; // azul oscuro - Muy Húmedo
            }
        }

        public string IconoEstado
        {
            get
            {
                if (Promedio < humedad_seco) return "🏜️";
                if (Promedio <= humedad_optimo) return "✅";
                if (Promedio <= humedad_humedo) return "💧";
                return "🌧️";
            }
        }

        public HistorialHumedadExtended() { }

        public HistorialHumedadExtended(HistorialHumedad historial)
        {
            HistorialHumedadId = historial.HistorialHumedadId;
            Fecha = historial.Fecha;
            Min = historial.Min;
            Max = historial.Max;
            Promedio = historial.Promedio;
            PesoAprox = historial.PesoAprox;
            PedidoId = historial.PedidoId;
            Pedido = historial.Pedido;
        }
    }
}