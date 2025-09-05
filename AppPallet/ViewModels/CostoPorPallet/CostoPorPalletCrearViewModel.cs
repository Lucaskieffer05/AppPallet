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
        public ObservableCollection<Empresa> listEmpresas = new();

        [ObservableProperty]
        public Empresa empresaIngresada = new();

        [ObservableProperty]
        private int mesIngresado;

        [ObservableProperty]
        private int añoIngresado;

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

            if(MesIngresado < 1 || MesIngresado > 12 || AñoIngresado < 1)
            {
                await MostrarAlerta("Error", "Por favor, ingrese un mes (1-12) y un año válidos.");
                return;
            }

            var mesDb = await _context.Mes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FechaMes.Month == MesIngresado && m.FechaMes.Year == AñoIngresado);

            if (mesDb == null)
            {
                await MostrarAlerta("Error", "No se encontró el mes ingresado en la base de datos.");
                return;
            }

            CostoPorPalletCreated.MesId = mesDb.MesId;
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
                CostoPorPalletCreated.MesId <= 0 ||
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
