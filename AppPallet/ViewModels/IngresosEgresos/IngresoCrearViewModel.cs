using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class IngresoCrearViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        private IngresoController _ingresoController;

        [ObservableProperty]
        public Ingreso ingresoCreated;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public IngresoCrearViewModel(IngresoController ingresoController)
        {
            _ingresoController = ingresoController;
            IngresoCreated = new Ingreso
            {
                Fecha = DateTime.Today
            };
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

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
        async Task CrearIngreso()
        {
            if (IngresoCreated == null || !ValidarIngreso(IngresoCreated))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            try
            {

                MessageResult resultado = await _ingresoController.CreateIngreso(IngresoCreated);

                await MostrarAlerta(resultado.Title, resultado.Message);

                IngresoCreated = new Ingreso();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
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


        private bool ValidarIngreso(Ingreso ingreso)
        {
            if (ingreso == null)
                return false;
            if (string.IsNullOrWhiteSpace(ingreso.DescripIngreso))
                return false;
            if (ingreso.Monto <= 0)
                return false;
            if (!ingreso.Fecha.HasValue)
                return false;
            return true;
        }
    }
}
