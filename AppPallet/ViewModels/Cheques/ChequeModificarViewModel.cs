using AppPallet.Constants;
using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppPallet.ViewModels
{
    public partial class ChequeModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        public Cheque? chequeSeleccionado;

        [ObservableProperty]
        public int estadoSeleccionado;
        public List<int> OpcionesEstado { get; } = new List<int> { 0, 1, 2, 3, 4 };

        public Dictionary<int, string> DescripcionesEstado { get; } = new Dictionary<int, string>
        {
            { 0, "Pendiente" },
            { 1, "En proceso" },
            { 2, "Pagado" },
            { 3, "Rechazado" },
            { 4, "Anulado" }
        };


        readonly IPopupService _popupService;

        readonly ChequeController _chequeController;

        readonly EgresoController _egresoController;

        readonly ActivoPasivoController _activoPasivoController;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeModificarViewModel(IPopupService popupService, ChequeController chequeController, EgresoController egresoController, ActivoPasivoController activoPasivoController)
        {
            _popupService = popupService;
            _chequeController = chequeController;
            _egresoController = egresoController;
            _activoPasivoController = activoPasivoController;
            EstadoSeleccionado = 0; 
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task CancelarPopup()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand]
        async Task GuardarPopup()
        {
            if (ChequeSeleccionado == null || !ValidarCheque())
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            // Guardar estado anterior antes de actualizar
            int? estadoAnteriorTemp = ChequeSeleccionado.Estado;
            ChequeSeleccionado.Estado = EstadoSeleccionado;

            try
            {
                // Buscar coincidencias en Egresos y Pasivos antes de actualizar
                string descripcionCheque = $"Cheque-{ChequeSeleccionado.Proveedor}-{ChequeSeleccionado.NumCheque ?? "Sin número"}";
                
                // Buscar en Egresos
                var egresoEncontrado = await _egresoController.BuscarEgresoPorCheque(descripcionCheque, ChequeSeleccionado.Monto, ChequeSeleccionado.FechaPago);
                
                if (egresoEncontrado != null)
                {
                    var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (mainPage != null)
                    {
                        string detallesEgreso = $"Descripción: {egresoEncontrado.DescripEgreso}\n" +
                                                $"Monto: ${egresoEncontrado.Monto:N2}\n" +
                                                $"Fecha: {egresoEncontrado.Fecha:dd/MM/yyyy}\n" +
                                                $"Comentario: {egresoEncontrado.Comentario ?? "Sin comentario"}";
                        
                        bool modificarEgreso = await mainPage.DisplayAlert(
                            "Coincidencia encontrada",
                            $"Se ha encontrado coincidencia con un cheque en egresos, ¿desea modificar su estado?\n\n{detallesEgreso}",
                            "Sí", "No");
                        
                        if (modificarEgreso)
                        {
                            string nuevoComentario = (EstadoSeleccionado == 1) ? "Pagado" : "Sin pagar";
                            // Crear una copia con el nuevo comentario para actualizar
                            Egreso egresoParaActualizar = new Egreso
                            {
                                EgresoId = egresoEncontrado.EgresoId,
                                DescripEgreso = egresoEncontrado.DescripEgreso,
                                Fecha = egresoEncontrado.Fecha,
                                Monto = egresoEncontrado.Monto,
                                Factura = egresoEncontrado.Factura,
                                SumaIva = egresoEncontrado.SumaIva,
                                Mes = egresoEncontrado.Mes,
                                Comentario = nuevoComentario
                            };
                            bool resultadoEgreso = await _egresoController.UpdateEgreso(egresoParaActualizar);
                            if (resultadoEgreso)
                            {
                                await MostrarAlerta("Éxito", "Egreso actualizado correctamente");
                            }
                            else
                            {
                                await MostrarAlerta("Error", "No se pudo actualizar el egreso");
                            }
                        }
                    }
                }

                // Buscar en Pasivos
                var pasivoEncontrado = await _activoPasivoController.BuscarPasivoPorCheque(descripcionCheque, ChequeSeleccionado.Monto, ChequeSeleccionado.FechaPago);
                
                if (pasivoEncontrado != null)
                {
                    var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (mainPage != null)
                    {
                        string detallesPasivo = $"ID: {pasivoEncontrado.ActivoPasivoId}\n" +
                                                $"Descripción: {pasivoEncontrado.Descripcion}\n" +
                                                $"Monto: ${pasivoEncontrado.Monto:N2}\n" +
                                                $"Fecha: {pasivoEncontrado.Fecha:dd/MM/yyyy}\n" +
                                                $"Estado: {pasivoEncontrado.Estado ?? "Sin estado"}";
                        
                        bool modificarPasivo = await mainPage.DisplayAlert(
                            "Coincidencia encontrada",
                            $"Se ha encontrado coincidencia con un cheque en pasivos, ¿desea modificar su estado?\n\n{detallesPasivo}",
                            "Sí", "No");
                        
                        if (modificarPasivo)
                        {
                            string nuevoEstado = (EstadoSeleccionado == 1) ? "Pagado" : "Sin Pagar";
                            // Crear una copia con el nuevo estado para actualizar
                            ActivoPasivo pasivoParaActualizar = new ActivoPasivo
                            {
                                ActivoPasivoId = pasivoEncontrado.ActivoPasivoId,
                                Descripcion = pasivoEncontrado.Descripcion,
                                Fecha = pasivoEncontrado.Fecha,
                                Mes = pasivoEncontrado.Mes,
                                Monto = pasivoEncontrado.Monto,
                                Categoria = pasivoEncontrado.Categoria,
                                Estado = nuevoEstado
                            };
                            var resultadoPasivo = await _activoPasivoController.UpdateActivoPasivo(pasivoParaActualizar);
                            if (resultadoPasivo.Title == MessageConstants.Titles.Success)
                            {
                                await MostrarAlerta("Éxito", "Pasivo actualizado correctamente");
                            }
                            else
                            {
                                await MostrarAlerta("Error", resultadoPasivo.Message);
                            }
                        }
                    }
                }

                var resultado = await _chequeController.UpdateCheque(ChequeSeleccionado);

                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Cheque actualizado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo actualizar el cheque");
                }
                ChequeSeleccionado = new Cheque();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }

            await CancelarPopup();
        }

        [RelayCommand]
        async Task EliminarCheque()
        {
            if (ChequeSeleccionado == null || ChequeSeleccionado.ChequeId == 0)
            {
                await MostrarAlerta("Error", "Datos inválidos");
                return;
            }

            // Obtener la página principal de forma compatible con la nueva API
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null)
            {
                await MostrarAlerta("Error", "No se pudo obtener la página principal.");
                return;
            }

            // Confirmar eliminación
            bool confirmar = await mainPage.DisplayAlert("Confirmar", "¿Está seguro de que desea eliminar este cheque?", "Sí", "No");
            if (!confirmar)
                return;

            try
            {
                var resultado = await _chequeController.DeleteCheque(ChequeSeleccionado.ChequeId);
                if (resultado)
                {
                    await MostrarAlerta("Éxito", "Cheque eliminado correctamente");
                }
                else
                {
                    await MostrarAlerta("Error", "No se pudo eliminar el cheque");
                }
                ChequeSeleccionado = new Cheque();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", ex.Message);
            }
            await CancelarPopup();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            ChequeSeleccionado = (Cheque)query["ChequeSeleccionado"];
            // Establecer el estado seleccionado basado en el estado del cheque
            if (ChequeSeleccionado != null && ChequeSeleccionado.Estado.HasValue)
            {
                EstadoSeleccionado = ChequeSeleccionado.Estado.Value;
            }
        }

        bool ValidarCheque()
        {
            if (ChequeSeleccionado == null) return false;
            if (ChequeSeleccionado.NumCheque == null || ChequeSeleccionado.NumCheque.Trim() == "") return false;
            if (ChequeSeleccionado.Proveedor == null || ChequeSeleccionado.Proveedor.Trim() == "") return false;
            if (ChequeSeleccionado.Tipo == null || ChequeSeleccionado.Tipo.Trim() == "") return false;
            if (ChequeSeleccionado.Monto <= 0) return false;
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
}
