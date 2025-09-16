using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppPallet.ViewModels
{
    public partial class CostoPorPalletCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones -------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        public CostoPorPallet costoPorPalletCreated;

        [ObservableProperty]
        public ObservableCollection<CostoPorCamion> listCostoPorCamions = new();

        [ObservableProperty]
        public ObservableCollection<Pallet> listPallets = new();

        [ObservableProperty]
        public Pallet palletIngresado = new();

        [ObservableProperty]
        public ObservableCollection<Empresa> listEmpresas = [];

        [ObservableProperty]
        public Empresa empresaIngresada = new();

        [ObservableProperty]
        private int mesIngresado;

        [ObservableProperty]
        private int añoIngresado;


        public ObservableCollection<string> Meses { get; } = new()
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

        public ObservableCollection<int> Años { get; } = new()
            {
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            };

        readonly CostoPorPalletController _costoPorPalletController;

        readonly PalletContext _context = new();

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public CostoPorPalletCrearViewModel(CostoPorPalletController controller)
        {
            ListPallets = _context.Pallets.AsNoTracking().ToList().ToObservableCollection();
            ListEmpresas = _context.Empresas.AsNoTracking().ToList().ToObservableCollection();
            _costoPorPalletController = controller;
            CostoPorPalletCreated = new CostoPorPallet();
            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;
            
            if (ListEmpresas.Count > 0)
                EmpresaIngresada = ListEmpresas.FirstOrDefault()!;

            if (ListPallets.Count > 0)
                PalletIngresado = ListPallets.FirstOrDefault()!;

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------


        [RelayCommand]
        public Task AgregarCostoPorCamion()
        {
            ListCostoPorCamions.Add(new CostoPorCamion
            {
                NombreCosto = string.Empty,
                Monto = 0
            });
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task CrearCostoPorPallet()
        {
            // Buscar el mes en la base de datos

            CostoPorPalletCreated.Mes = new DateTime(AñoIngresado, MesIngresado+1, 1);

            CostoPorPalletCreated.EmpresaId = EmpresaIngresada.EmpresaId;
            CostoPorPalletCreated.PalletId = PalletIngresado.PalletId;

            if (ListCostoPorCamions.Count == 0)
            {
                await MostrarAlerta("Error", "Debe agregar al menos un costo por camión.");
                return;
            }

            CostoPorPalletCreated.CostoPorCamions = ListCostoPorCamions;

            if (string.IsNullOrWhiteSpace(CostoPorPalletCreated.NombrePalletCliente) ||
                CostoPorPalletCreated.CantidadPorDia <= 0 ||
                CostoPorPalletCreated.CargaCamion <= 0 ||
                CostoPorPalletCreated.PalletId <= 0 ||
                CostoPorPalletCreated.EmpresaId <= 0)
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos obligatorios.");
                return;
            }
            try
            {
                
                // Guardar el CostoPorPallet
                var resultado = await _costoPorPalletController.CreateCostoPorPallet(CostoPorPalletCreated);

                if (!resultado)
                {
                    await MostrarAlerta("Error", "Ocurrió un error al guardar el costo por pallet.");
                    return;
                }

                await MostrarAlerta("Éxito", "Costo por pallet y costos por camión guardados correctamente.");

                CostoPorPalletCreated = new CostoPorPallet();
                ListCostoPorCamions.Clear();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error inesperado: {ex.Message}");
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
