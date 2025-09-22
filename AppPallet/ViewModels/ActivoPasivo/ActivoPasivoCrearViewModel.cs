using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using Azure.Messaging;
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
    public partial class ActivoPasivoCrearViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------
        readonly ActivoPasivoController _activoPasivoController;

        [ObservableProperty]
        public ActivoPasivo activoPasivoCreated;

        [ObservableProperty]
        private int mesIngresado = DateTime.Today.Month - 1;

        [ObservableProperty]
        private int añoIngresado = DateTime.Today.Year;

        [ObservableProperty]
        private int quincenaIngresada = 0;

        public ObservableCollection<string> Meses { get; } =
            [
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            ];

        public ObservableCollection<int> Años { get; } =
            [
                DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1
            ];

        public ObservableCollection<string> Categorias { get; } =
            [
                "Activo", "Pasivo"
            ];

        public ObservableCollection<string> Quincenas { get; } =
            [
                "Primera Quincena", "Segunda Quincena"
            ];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ActivoPasivoCrearViewModel(ActivoPasivoController activoPasivoController)
        {
            _activoPasivoController = activoPasivoController;
            activoPasivoCreated = new ActivoPasivo
            {
                Fecha = DateTime.Today,
                Mes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Monto = 0,
                Descripcion = string.Empty,
                Categoria = "Activo" // Valor por defecto
            };
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
        async Task CrearActivoPasivo()
        {
            if (!ValidarActivoPasivo(ActivoPasivoCreated))
            {
                await MostrarAlerta("Error", "Por favor, complete todos los campos correctamente.");
                return;
            }
            if(QuincenaIngresada == 0)
            {
                ActivoPasivoCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 1);
            }
            else
            {
                ActivoPasivoCreated.Mes = new DateTime(AñoIngresado, MesIngresado + 1, 16);
            }
            MessageResult result = await _activoPasivoController.CreateActivoPasivo(ActivoPasivoCreated);
            await MostrarAlerta(result.Title, result.Message);

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


        private bool ValidarActivoPasivo(ActivoPasivo activoPasivo)
        {
            if (string.IsNullOrWhiteSpace(activoPasivo.Descripcion))
                return false;
            if (activoPasivo.Monto < 0)
                return false;
            if (string.IsNullOrWhiteSpace(activoPasivo.Categoria))
                return false;
            if (activoPasivo.Mes == default)
                return false;
            return true;
        }

    }
}
