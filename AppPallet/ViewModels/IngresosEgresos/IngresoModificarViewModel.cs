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
    public partial class IngresoModificarViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private IngresoController _ingresoController;

        [ObservableProperty]
        public Ingreso? ingresoModificar;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public IngresoModificarViewModel(IngresoController ingresoController)
        {
            _ingresoController = ingresoController;
        }



        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Ingreso", out var ingreso) && ingreso is Ingreso _ingreso)
            {
                IngresoModificar = _ingreso;
            }

        }


        [RelayCommand]
        async Task VolverAtras()
        {
            //Volver atras
            await Shell.Current.GoToAsync("..");
        }



        [RelayCommand]
        async Task ModificarIngreso()
        {
            if (IngresoModificar == null || !ValidarIngreso(IngresoModificar))
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                bool exito = await _ingresoController.UpdateIngreso(IngresoModificar);
                if (exito)
                {
                    await MostrarAlerta("Éxito", "Ingreso modificado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo modificar el ingreso");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Ocurrió un error: {ex.Message}");
            }

            await VolverAtras();
        }

        [RelayCommand]
        async Task EliminarIngreso()
        {
            if (IngresoModificar == null || IngresoModificar.IngresoId <= 0)
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }
            try
            {
                bool exito = await _ingresoController.DeleteIngreso(IngresoModificar.IngresoId);
                if (exito)
                {
                    await MostrarAlerta("Éxito", "Ingreso eliminado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el ingreso");
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
