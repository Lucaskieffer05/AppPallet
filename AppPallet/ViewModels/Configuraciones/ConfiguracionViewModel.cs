using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;

namespace AppPallet.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        // Constantes para las keys de Preferences
        private const string ConnectionStringKey = "database_connection_string";
        private const string IvaPercentageKey = "iva_percentage";
        private const string HumedadSecoKey = "humedad_seco";
        private const string HumedadOptimoKey = "humedad_optimo";
        private const string HumedadHumedoKey = "humedad_humedo";

        // Valores por defecto
        private const string DefaultConnectionString = "Server=localhost;Database=AppPallet;Trusted_Connection=true;";
        private const double DefaultIvaPercentage = 21.0;
        private const int DefaultHumedadSeco = 12;
        private const int DefaultHumedadOptimo = 18;
        private const int DefaultHumedadHumedo = 22;

        [ObservableProperty]
        private string connectionString = string.Empty;

        [ObservableProperty]
        private double ivaPercentage;

        [ObservableProperty]
        private int humedadSeco;

        [ObservableProperty]
        private int humedadOptimo;

        [ObservableProperty]
        private int humedadHumedo;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string mensajeEstado = string.Empty;

        [ObservableProperty]
        private bool mostrarMensajePruebaConexion;

        [ObservableProperty]
        private bool mostrarMensajeExito;

        public ConfiguracionViewModel()
        {
            CargarConfiguraciones();
        }

        [RelayCommand]
        public void CargarConfiguraciones()
        {
            try
            {
                IsBusy = true;

                // Cargar configuración de base de datos
                ConnectionString = Preferences.Get(ConnectionStringKey, DefaultConnectionString);

                // Cargar configuración de IVA
                IvaPercentage = Preferences.Get(IvaPercentageKey, DefaultIvaPercentage);

                // Cargar configuración de humedad
                HumedadSeco = Preferences.Get(HumedadSecoKey, DefaultHumedadSeco);
                HumedadOptimo = Preferences.Get(HumedadOptimoKey, DefaultHumedadOptimo);
                HumedadHumedo = Preferences.Get(HumedadHumedoKey, DefaultHumedadHumedo);

                MensajeEstado = "Configuraciones cargadas correctamente";
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar configuraciones: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GuardarConfiguraciones()
        {
            if (!ValidarConfiguraciones())
            {
                await MostrarAlerta("Error", "Por favor, corrija los errores antes de guardar.");
                return;
            }

            try
            {
                IsBusy = true;

                // Guardar configuración de base de datos
                Preferences.Set(ConnectionStringKey, ConnectionString);

                // Guardar configuración de IVA
                Preferences.Set(IvaPercentageKey, IvaPercentage);

                // Guardar configuración de humedad
                Preferences.Set(HumedadSecoKey, HumedadSeco);
                Preferences.Set(HumedadOptimoKey, HumedadOptimo);
                Preferences.Set(HumedadHumedoKey, HumedadHumedo);

                MensajeEstado = "✅ Configuraciones guardadas correctamente";
                MostrarMensajeExito = true;

                // Ocultar mensaje después de 3 segundos
                await Task.Delay(3000);
                MostrarMensajeExito = false;
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudieron guardar las configuraciones: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void RestablecerValoresPorDefecto()
        {
            ConnectionString = DefaultConnectionString;
            IvaPercentage = DefaultIvaPercentage;
            HumedadSeco = DefaultHumedadSeco;
            HumedadOptimo = DefaultHumedadOptimo;
            HumedadHumedo = DefaultHumedadHumedo;

            MensajeEstado = "Valores por defecto restablecidos";
        }

        [RelayCommand]
        public async Task ProbarConexion()
        {
            // Aquí podrías implementar una prueba de conexión a la base de datos
            MensajeEstado = "🔄 Probando conexión...";
            MostrarMensajePruebaConexion = true;

            try
            {
                IsBusy = true;
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        MensajeEstado = "✅ Conexión exitosa a la base de datos";
                    }
                    else
                    {
                        MensajeEstado = "❌ No se pudo establecer la conexión";
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"❌ Error de conexión: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await Task.Delay(3000);
                MostrarMensajePruebaConexion = false;
            }


        }

        private bool ValidarConfiguraciones()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                MensajeEstado = "❌ La cadena de conexión no puede estar vacía";
                return false;
            }

            if (IvaPercentage < 0 || IvaPercentage > 100)
            {
                MensajeEstado = "❌ El porcentaje de IVA debe estar entre 0 y 100";
                return false;
            }

            if (HumedadSeco >= HumedadOptimo || HumedadOptimo >= HumedadHumedo)
            {
                MensajeEstado = "❌ Los valores de humedad deben ser: Seco < Óptimo < Húmedo";
                return false;
            }

            if (HumedadSeco < 0 || HumedadOptimo < 0 || HumedadHumedo < 0)
            {
                MensajeEstado = "❌ Los valores de humedad no pueden ser negativos";
                return false;
            }

            return true;
        }

        // Propiedades calculadas para mostrar información de validación
        public string RangoHumedadInfo =>
            $"Rangos actuales: Seco (< {HumedadSeco}%) | Óptimo ({HumedadSeco}-{HumedadOptimo}%) | Húmedo ({HumedadOptimo}-{HumedadHumedo}%) | Muy Húmedo (> {HumedadHumedo}%)";

        public string IvaInfo => $"IVA aplicado: {IvaPercentage}%";

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