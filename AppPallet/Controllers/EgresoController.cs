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
        public async Task<List<Egreso>> GetAllEgresos(int mes)
        {
            try
            {
                return await _context.Egresos.AsNoTracking().Where(i => i.Fecha.HasValue && i.Fecha.Value.Month == mes).OrderBy(i => i.Fecha).ToListAsync();
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
                _context.Egresos.Add(nuevoEgreso);
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
                var egreso = await _context.Egresos.Where(c => c.EgresoId == egresoModificado.EgresoId).FirstOrDefaultAsync();
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


    }
}
