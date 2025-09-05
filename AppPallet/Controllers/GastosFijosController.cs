using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class GastosFijosController
    {

        private readonly PalletContext _context;

        public GastosFijosController(PalletContext context)
        {
            _context = context;
        }

        // ...resto del código...

        public async Task<List<GastosFijo>> GetAllGastosFijos()
        {
            try
            {
                return await _context.GastosFijos
                    .Include(g => g.Mes)
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Crear un nuevo cheque
        public async Task<bool> CreateGastoFijo(GastosFijo nuevoGastoFijo)
        {
            try
            {
                var mesId = _context.Mes.Where(m => m.FechaMes.Month == nuevoGastoFijo.Mes.FechaMes.Month && m.FechaMes.Year == nuevoGastoFijo.Mes.FechaMes.Year)
                                        .Select(m => m.MesId)
                                        .FirstOrDefault();

                nuevoGastoFijo.MesId = mesId;

                _context.GastosFijos.Add(nuevoGastoFijo);

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

        // Modificar un cheque existente
        public async Task<bool> UpdateGastoFijo(GastosFijo gastoFijoModificado)
        {
            try
            {
                var gastoFijo = await _context.GastosFijos.Where(g => g.GastosFijosId == gastoFijoModificado.GastosFijosId).FirstOrDefaultAsync();
                if (gastoFijo == null)
                    return false;

                gastoFijo.NombreGastoFijo = gastoFijoModificado.NombreGastoFijo;
                gastoFijo.Monto = gastoFijoModificado.Monto;

                _context.GastosFijos.Update(gastoFijo);

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
