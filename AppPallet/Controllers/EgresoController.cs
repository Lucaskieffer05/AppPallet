using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class EgresoController
    {

        private readonly PalletContext _context;

        public EgresoController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los egresos
        public async Task<List<Egreso>> GetAllEgresos(DateTime mes)
        {
            try
            {
                return await _context.Egreso.AsNoTracking().Where(c => c.Mes.HasValue && c.Mes.Value.Month == mes.Month && c.Mes.Value.Year == mes.Year).OrderByDescending(c => c.Fecha).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Egreso>();
            }
        }

        // Crear un nuevo egreso
        public async Task<bool> CreateEgreso(Egreso nuevoEgreso)
        {
            try
            {
                // crear el nuevo egreso
                Egreso egreso = new Egreso
                {
                    DescripEgreso = nuevoEgreso.DescripEgreso,
                    Fecha = nuevoEgreso.Fecha,
                    Factura = nuevoEgreso.Factura,
                    Monto = nuevoEgreso.Monto,
                    SumaIva = nuevoEgreso.SumaIva,
                    Mes = nuevoEgreso.Mes,
                    Comentario = nuevoEgreso.Comentario
                };


                _context.Egreso.Add(nuevoEgreso);
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

        // Modificar un egreso existente
        public async Task<bool> UpdateEgreso(Egreso egresoModificado)
        {
            try
            {
                var egreso = await _context.Egreso.Where(c => c.EgresoId == egresoModificado.EgresoId).FirstOrDefaultAsync();
                if (egreso == null)
                {
                    Console.WriteLine("Egreso no encontrado.");
                    return false;
                }
                // Actualizar los campos del egreso
                egreso.Fecha = egresoModificado.Fecha;
                egreso.Monto = egresoModificado.Monto;
                egreso.DescripEgreso = egresoModificado.DescripEgreso;
                egreso.Factura = egresoModificado.Factura;
                egreso.SumaIva = egresoModificado.SumaIva;
                egreso.Mes = egresoModificado.Mes;
                egreso.Comentario = egresoModificado.Comentario;

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

        // Eliminar un egreso
        public async Task<bool> DeleteEgreso(int egresoId)
        {
            try
            {
                var egreso = await _context.Egreso.FindAsync(egresoId);
                if (egreso == null)
                {
                    Console.WriteLine("Egreso no encontrado.");
                    return false;
                }
                _context.Egreso.Remove(egreso);
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
