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

        public async Task<List<GastosFijo>> GetAllGastosFijos(DateTime mes)
        {
            try
            {
                return await _context.GastosFijos
                    .AsNoTracking()
                    .Where(g => g.Mes.Year == mes.Year && g.Mes.Month == mes.Month)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        //Obtener totales de gasto fijo por mes
        public async Task<List<TotalGastoFijoPorMes>> GetTotalGastoFijoPorMes()
        {
            try
            {
                var result = await _context.GastosFijos
                    .GroupBy(g => g.Mes)
                    .Select(g => new TotalGastoFijoPorMes
                    {
                        Mes = g.Key,
                        TotalGastoFijo = g.Sum(x => x.Monto)
                    })
                    .OrderBy(t => t.Mes)
                    .AsNoTracking()
                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<TotalGastoFijoPorMes>();
            }
        }

        // Crear un nuevo cheque
        public async Task<bool> CreateGastoFijo(GastosFijo nuevoGastoFijo)
        {
            try
            {

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
                gastoFijo.Mes = gastoFijoModificado.Mes;

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

        // Eliminar un cheque por su ID
        public async Task<bool> DeleteGastoFijo(int gastoFijoId)
        {
            try
            {
                var gastoFijo = await _context.GastosFijos.Where(g => g.GastosFijosId == gastoFijoId).FirstOrDefaultAsync();
                if (gastoFijo == null)
                    return false;
                _context.GastosFijos.Remove(gastoFijo);
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
