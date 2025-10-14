using AppPallet.Controllers;
using AppPallet.Models;
using AppPallet.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class PresupuestoMostrarViewModel : ObservableObject, IQueryAttributable
    {


        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly EmpresaController _empresaController;
        readonly GastosFijosController _gastosFijosController;
        readonly CostoPorPalletController _costoPorPalletController;

        private int EmpresaId;
        public List<TotalGastoFijoPorMesDTO> TotalGastoFijoPorMesList;

        [ObservableProperty]
        private bool noHayPresu;

        [ObservableProperty]
        public Empresa? empresaSeleccionada;

        [ObservableProperty]
        public List<GastosYCostosDTO> resultado;

        [ObservableProperty]
        public string titulo;


        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;


        public ObservableCollection<string> FiltroMeses { get; } = new()
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

        public ObservableCollection<int> FiltroAños { get; } = new()
            {
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            };


        public ObservableCollection<string> Meses { get; } =
            [
                "01", "02", "03", "04", "05", "06",
                "07", "08", "09", "10", "11", "12"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];


        // Inicializa los campos en el constructor para evitar el error CS8618
        public PresupuestoMostrarViewModel(EmpresaController empresaController, GastosFijosController gastosFijosController, CostoPorPalletController costoPorPalletController)
        {
            _empresaController = empresaController;
            Titulo = "PRESUPUESTO";
            _gastosFijosController = gastosFijosController;
            _costoPorPalletController = costoPorPalletController;

            TotalGastoFijoPorMesList = [];
            Resultado = [];
        }



        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        private async Task RecargarResultado(int empresaId)
        {
            if (empresaId <= 0)
            {
                await MostrarAlerta("Error", "ID de empresa inválido.");
                return;
            }

            // Vuelve a cargar los datos actualizados
            EmpresaSeleccionada = await _empresaController.GetEmpresaById(empresaId, new DateTime(AñoIngresado, MesIngresado + 1, 1));
            TotalGastoFijoPorMesList = await _gastosFijosController.GetTotalGastoFijoPorMes();

            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Error", "No se encontró la empresa seleccionada.");
                return;
            }
            
            Titulo = $"PRESUPUESTO - {EmpresaSeleccionada.NomEmpresa}";

            Resultado = EmpresaSeleccionada.CostoPorPallet
                .Select(c => new GastosYCostosDTO
                {
                    Costo = c,
                    TotalGasto = TotalGastoFijoPorMesList.FirstOrDefault(t => t.Mes.Month == c.Mes.Month) ?? new TotalGastoFijoPorMesDTO { Mes = c.Mes, TotalGastoFijo = 0 },
                })
                .ToList();

            NoHayPresu = Resultado.Count == 0;

            // bandera para saber si se detecto un cambio en gastos fijos total y por lo tanto en los costos finales de los pallets
            bool huboCambiosEnGastosFijos = false;
            // actualizar ganancias
            foreach (var item in Resultado)
            {
                if (item.CostoFinalPallet > 0 && item.Costo != null)
                {
                    item.Costo.GananciaPorCantPallet = (int?)(item.Costo.PrecioPallet - item.CostoFinalPallet);
                    bool response = await _costoPorPalletController.UpdatePrecioCostoPorPallet(item.Costo);
                    if (response)
                    {
                        huboCambiosEnGastosFijos = true;
                    }
                }
            }

            Resultado = Resultado.ToList();

            //Notificar que hubo cambios
            if (huboCambiosEnGastosFijos)
            {
                await MostrarAlerta("Éxito", "Los costos finales y ganancias se han actualizado debido a cambios en los gastos fijos.");
            }



        }

        partial void OnMesIngresadoChanged(int oldValue, int newValue)
        {
            async void LoadAsync()
            {
                try
                {
                    await RecargarResultado(EmpresaId);
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
                    await RecargarResultado(EmpresaId);
                }
                catch (Exception ex)
                {
                    await MostrarAlerta("Error", $"Error al cargar la lista de gastos fijos: {ex.Message}");
                }
            }

            LoadAsync();
        }



        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("EmpresaId", out var id) && id is int empresaId)
            {
                EmpresaId = empresaId;
                await RecargarResultado(empresaId);
            }

        }

        [RelayCommand]
        public async Task MostrarPresupuesto(GastosYCostosDTO gastosYCostos)
        {
            await Shell.Current.GoToAsync(
                nameof(PresupuestoModificarView),
                new Dictionary<string, object>
                {
                    { "gastosYCostos", gastosYCostos }
                });
        }

        [RelayCommand]
        public async Task GuardarPrecio(GastosYCostosDTO gastosYCostos)
        {
            // guardar solo el precio del pallet
            if (gastosYCostos.Costo != null)
            {
                if(gastosYCostos.CostoFinalPallet > 0)
                {
                    gastosYCostos.Costo.GananciaPorCantPallet = (int?)(gastosYCostos.Costo.PrecioPallet - gastosYCostos.CostoFinalPallet);
                }
                bool response = await _costoPorPalletController.UpdatePrecioCostoPorPallet(gastosYCostos.Costo);
            }

            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Error", "No hay una empresa seleccionada.");
                return;
            }

            await RecargarResultado(EmpresaSeleccionada.EmpresaId);


        }

        [RelayCommand]
        public async Task CopiarPresupuesto(GastosYCostosDTO gastosYCostosToCopy)
        {
            if (gastosYCostosToCopy.Costo == null)
            {
                await MostrarAlerta("Error", "No hay un costo para copiar.");
                return;
            }
            CostoPorPallet costoToCopy = gastosYCostosToCopy.Costo;

            // Preguntar al usuario si desea copiar el presupuesto
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }
            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Desea copiar este presupuesto al mes seleccionado?", "Sí", "No");
            if (!confirmar)
                return;

            // verificar que AñoToCopyIndex tenga valor
            if (gastosYCostosToCopy.AñoToCopyIndex < 0 || gastosYCostosToCopy.AñoToCopyIndex >= Años.Count || gastosYCostosToCopy.MesToCopy < 0)
            {
                await MostrarAlerta("Error", "Año o Mes inválido para copiar el presupuesto.");
                return;
            }

            gastosYCostosToCopy.AñoToCopy = Años[gastosYCostosToCopy.AñoToCopyIndex];

            bool confirmar2 = await mainPage.DisplayAlert("Confirmar", $"El presupuesto {costoToCopy.NombrePalletCliente} - precio: ${costoToCopy.PrecioPallet}" +
                                                                       $" del mes {costoToCopy.Mes.Month}-{costoToCopy.Mes.Year} será copiado al mes {gastosYCostosToCopy.MesToCopy + 1}-{gastosYCostosToCopy.AñoToCopy} " +
                                                                       $"¿Desea ejecutar esta acción?", "Sí", "No");
            if (!confirmar2)
                return;



            // Crear una copia del costo por pallet
            var nuevoCosto = new CostoPorPallet
            {
                NombrePalletCliente = "Copia - " + costoToCopy.NombrePalletCliente,
                EmpresaId = costoToCopy.EmpresaId,
                PalletId = costoToCopy.PalletId,
                Mes = new DateTime(gastosYCostosToCopy.AñoToCopy, gastosYCostosToCopy.MesToCopy + 1, 1),
                CargaCamion = costoToCopy.CargaCamion,
                CantidadPorDia = costoToCopy.CantidadPorDia,
                HorasPorMes = costoToCopy.HorasPorMes,
                PrecioPallet = costoToCopy.PrecioPallet,
                CostoPorCamion = costoToCopy.CostoPorCamion
                    .Select(c => new CostoPorCamion
                    {
                        NombreCosto = c.NombreCosto,
                        Monto = c.Monto
                    })
                    .ToList()
            };
            bool response = await _costoPorPalletController.CreateCostoPorPallet(nuevoCosto);
            if (!response)
            {
                await MostrarAlerta("Error", "No se pudo copiar el presupuesto.");
                return;
            }
            else
            {
                await MostrarAlerta("Éxito", "Presupuesto copiado correctamente.");
            }
            if (EmpresaSeleccionada == null)
            {
                await MostrarAlerta("Error", "No hay una empresa seleccionada.");
                return;
            }
            await RecargarResultado(EmpresaSeleccionada.EmpresaId);


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
