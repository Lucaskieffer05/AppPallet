using AppPallet.Constants;
using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class IngresoController
    {
        private readonly PalletContext _context;

        public IngresoController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los ingresos
        public async Task<List<Ingreso>> GetAllIngresos(DateTime mes)
        {
            try
            {
                return await _context.Ingreso.AsNoTracking().Where(c => c.Fecha.HasValue && c.Fecha.Value.Month == mes.Month && c.Fecha.Value.Year == mes.Year).OrderByDescending(c => c.Fecha).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Ingreso>();
            }
        }

        // Crear un nuevo ingreso
        public async Task<MessageResult> CreateIngreso(Ingreso nuevoIngreso)
        {
            try
            {
                // crear el nuevo ingreso
                Ingreso ingresoCreated = new()
                {
                    Fecha = nuevoIngreso.Fecha,
                    DescripIngreso = nuevoIngreso.DescripIngreso,
                    Op = nuevoIngreso.Op,
                    Remito = nuevoIngreso.Remito,
                    Factura = nuevoIngreso.Factura,
                    Monto = nuevoIngreso.Monto,
                    Comentario = nuevoIngreso.Comentario
                };


                _context.Ingreso.Add(ingresoCreated);
                var result = await _context.SaveChangesAsync();
                if(result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Ingreso.CreateSuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Ingreso.CreateError);
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Error de base de datos: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Error general: {ex.Message}");
            }
        }

        // Modificar un ingreso existente
        public async Task<bool> UpdateIngreso(Ingreso ingresoModificado)
        {
            try
            {
                var ingreso = await _context.Ingreso.Where(i => i.IngresoId == ingresoModificado.IngresoId).FirstOrDefaultAsync();
                if (ingreso == null)
                {
                    Console.WriteLine("Ingreso no encontrado.");
                    return false;
                }
                ingreso.Fecha = ingresoModificado.Fecha;
                ingreso.DescripIngreso = ingresoModificado.DescripIngreso;
                ingreso.Op = ingresoModificado.Op;
                ingreso.Remito = ingresoModificado.Remito;
                ingreso.Factura = ingresoModificado.Factura;
                ingreso.Monto = ingresoModificado.Monto;
                ingreso.Comentario = ingresoModificado.Comentario;
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error de base de datos: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteIngreso(int id)
        {
            try
            {
                var ingreso = await _context.Ingreso.Where(i => i.IngresoId == id).FirstOrDefaultAsync();
                if (ingreso == null)
                {
                    Console.WriteLine("Ingreso no encontrado.");
                    return false;
                }
                _context.Ingreso.Remove(ingreso);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error de base de datos: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }

        }
    }
}
