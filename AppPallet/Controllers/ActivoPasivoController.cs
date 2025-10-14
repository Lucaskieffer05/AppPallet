using AppPallet.Constants;
using AppPallet.Models;
using Azure.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class ActivoPasivoController
    {

        private readonly PalletContext _context;

        public ActivoPasivoController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los activos y pasivos
        public async Task<List<ActivoPasivo>> GetAllActivosPasivos(DateTime Mes)
        {
            try
            {
                return await _context.ActivoPasivo.AsNoTracking().Where(a => a.Mes.Month == Mes.Month && a.Mes.Year == Mes.Year).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ActivoPasivo>();
            }
        }

        // Crear un nuevo activo o pasivo
        public async Task<MessageResult> CreateActivoPasivo(ActivoPasivo nuevoActivoPasivo)
        {
            try
            {
                // crear el nuevo activo o pasivo
                ActivoPasivo activoPasivo = new ActivoPasivo
                {
                    Descripcion = nuevoActivoPasivo.Descripcion,
                    Fecha = nuevoActivoPasivo.Fecha,
                    Monto = nuevoActivoPasivo.Monto,
                    Mes = nuevoActivoPasivo.Mes,
                    Categoria = nuevoActivoPasivo.Categoria
                };

                _context.ActivoPasivo.Add(nuevoActivoPasivo);
                var result = await _context.SaveChangesAsync();
                
                if (result == 0)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.ActivoPasivo.CreateError);
                }

                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.ActivoPasivo.CreateSuccess);


            }
            catch (DbUpdateException)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.ActivoPasivo.CreateError);
            }
        }


        // Modificar
        public async Task<MessageResult> UpdateActivoPasivo(ActivoPasivo activoPasivoModificar)
        {
            try
            {
                var activopasivoExistente = await _context.ActivoPasivo.FindAsync(activoPasivoModificar.ActivoPasivoId);
                if (activopasivoExistente == null)
                {
                    // Devuelve un objeto anónimo con los mensajes requeridos
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.ActivoPasivo.NotFound);
                }

                // Comprobar si hay cambios
                bool hayCambios = activopasivoExistente.Fecha != activoPasivoModificar.Fecha ||
                                  activopasivoExistente.Mes != activoPasivoModificar.Mes ||
                                  activopasivoExistente.Descripcion != activoPasivoModificar.Descripcion ||
                                  activopasivoExistente.Monto != activoPasivoModificar.Monto ||
                                  activopasivoExistente.Estado != activoPasivoModificar.Estado ||
                                  activopasivoExistente.Categoria != activoPasivoModificar.Categoria;

                if (!hayCambios)
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.ActivoPasivo.ModifiedNoChanges) ;

                // Actualizar los campos del activo o pasivo
                activopasivoExistente.Fecha = activoPasivoModificar.Fecha;
                activopasivoExistente.Mes = activoPasivoModificar.Mes;
                activopasivoExistente.Descripcion = activoPasivoModificar.Descripcion;
                activopasivoExistente.Monto = activoPasivoModificar.Monto;
                activopasivoExistente.Categoria = activoPasivoModificar.Categoria;
                activopasivoExistente.Estado = activoPasivoModificar.Estado;

                var result = await _context.SaveChangesAsync();

                if (result == 0)
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.ActivoPasivo.ModifyError);

                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.ActivoPasivo.ModifiedSuccess);

            }
            catch (DbUpdateException)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.ActivoPasivo.ModifyError);
            }
        }

        // Eliminar
        public async Task<bool> DeleteActivoPasivo(int activoPasivoId)
        {
            try
            {
                var activoPasivo = await _context.ActivoPasivo.FindAsync(activoPasivoId);
                if (activoPasivo == null)
                {
                    Console.WriteLine("Activo o Pasivo no encontrado.");
                    return false;
                }
                _context.ActivoPasivo.Remove(activoPasivo);
                var result = await _context.SaveChangesAsync();
                return result > 0;

            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error de base de datos: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
                return false;
            }

        }

        public async Task<bool> ExistePasivoEnMes(string descripcion, decimal monto , DateTime mes)
        {
            try
            {
                return await _context.ActivoPasivo.AnyAsync(ap => ap.Descripcion == descripcion && ap.Monto== monto && ap.Mes.Month == mes.Month && ap.Mes.Year == mes.Year && ap.Categoria.ToLower() == "pasivo");

            }
            catch
            {
                return false;

            }
        }
    }
            
}
