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
        public async Task<List<Ingreso>> GetAllIngresos(int mes)
        {
            try
            {
                return await _context.Ingresos.AsNoTracking().Where(i => i.Fecha.HasValue && i.Fecha.Value.Month == mes).OrderBy(i => i.Fecha).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Ingreso>();
            }
        }

        // Crear un nuevo ingreso
        public async Task<bool> CreateIngreso(Ingreso nuevoIngreso)
        {
            try
            {
                _context.Ingresos.Add(nuevoIngreso);
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

        // Modificar un ingreso existente
        public async Task<bool> UpdateIngreso(Ingreso ingresoModificado)
        {
            try
            {
                var ingreso = await _context.Ingresos.Where(i => i.IngresoId == ingresoModificado.IngresoId).FirstOrDefaultAsync();
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


    }
}
