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
        public async Task<MessageResult> CreateEgreso(Egreso nuevoEgreso)
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
                if (result == 0)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Egreso.CreateError);
                }
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Egreso.CreateSuccess);
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalle: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalle: {ex.Message}");
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

        public async Task<bool> ExisteEgresoEnMes(string descripEgreso, decimal monto, DateTime? mes)
        {
            try
            {
                return await _context.Egreso.AnyAsync(e =>
                    e.DescripEgreso == descripEgreso &&
                    e.Monto == monto &&
                    e.Mes.HasValue &&
                    mes.HasValue &&
                    e.Mes.Value.Month == mes.Value.Month &&
                    e.Mes.Value.Year == mes.Value.Year &&
                    e.Comentario == "Gasto Fijo");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;

            }
        }

        public async Task<List<FinanzaMensualDTO>> GetEgresosAnuales(int year)
        {
            try
            {
                var egresosAnuales = await _context.Egreso
                    .Where(e => e.Mes.HasValue && e.Mes.Value.Year == year)
                    .GroupBy(e => e.Mes.Value.Month)
                    .Select(g => new FinanzaMensualDTO
                    {
                        Mes = g.Key,
                        TotalFinanza = g.Sum(e => e.Monto + (e.SumaIva ?? 0))
                    })
                    .ToListAsync();
                // Asegurarse de que todos los meses estén representados
                var resultadoCompleto = new List<FinanzaMensualDTO>();
                for (int mes = 1; mes <= 12; mes++)
                {
                    var finanzaMes = egresosAnuales.FirstOrDefault(f => f.Mes == mes);
                    if (finanzaMes != null)
                    {
                        resultadoCompleto.Add(finanzaMes);
                    }
                    else
                    {
                        resultadoCompleto.Add(new FinanzaMensualDTO { Mes = mes, TotalFinanza = 0 });
                    }
                }
                return resultadoCompleto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener egresos anuales: {ex.Message}");
                return new List<FinanzaMensualDTO>();

            }
        }

        // Buscar egreso por descripción de cheque, monto y fecha
        public async Task<Egreso?> BuscarEgresoPorCheque(string descripcionCheque, decimal monto, DateTime fechaPago)
        {
            try
            {
                // No usar AsNoTracking para que la entidad pueda ser modificada
                return await _context.Egreso
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => 
                        e.DescripEgreso == descripcionCheque &&
                        e.Monto == monto &&
                        e.Fecha.HasValue &&
                        e.Fecha.Value.Date == fechaPago.Date);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar egreso por cheque: {ex.Message}");
                return null;
            }
        }
    }
}
