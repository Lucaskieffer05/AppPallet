using AppPallet.Controllers;
using AppPallet.Models;
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
    public partial class EgresoCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private EgresoController _egresoController;

        [ObservableProperty]
        public Egreso egresoCreated;

        [ObservableProperty]
        public int mesIngresado;

        [ObservableProperty]
        public int añoIngresado;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public EgresoCrearViewModel(EgresoController egresoController)
        {
            _egresoController = egresoController;
            EgresoCreated = new Egreso
            {
                Fecha = DateTime.Today
            };

            MesIngresado = DateTime.Today.Month - 1;
            AñoIngresado = DateTime.Today.Year;
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task VolverAtras()
        {
            //Volver atras
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task CrearEgreso()
        {
            if (EgresoCreated == null || !ValidarEgreso(EgresoCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                EgresoCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);
                var resultado = await _egresoController.CreateEgreso(EgresoCreated);
                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Egreso creado correctamente");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo crear el egreso");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
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

        private bool ValidarEgreso(Egreso egreso)
        {
            if (egreso == null)
                return false;
            if (string.IsNullOrWhiteSpace(egreso.DescripEgreso))
                return false;
            if (egreso.Monto < 0) // Ejemplo de rango
                return false;
            if (egreso.Fecha > DateTime.Today)
                return false;
            return true;



        }
    }
}
