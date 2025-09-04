using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class ChequeController
    {
        private readonly PalletContext _context;

        public ChequeController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los cheques
        public async Task<List<Cheque>> GetAllCheques()
        {
            try
            {
                return await _context.Cheques.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Crear un nuevo cheque
        public async Task<bool> CreateCheque(Cheque nuevoCheque)
        {
            try
            {
                _context.Cheques.Add(nuevoCheque);
                var result = await _context.SaveChangesAsync();
                return result > 0; // SaveChangesAsync retorna el número de entradas afectadas
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
        public async Task<bool> UpdateCheque(Cheque chequeModificado)
        {
            try
            {
                var cheque = await _context.Cheques.Where(c => c.ChequeId == chequeModificado.ChequeId).FirstOrDefaultAsync();
                
                if(cheque == null)
                {
                    return false;
                }

                cheque.NumCheque = chequeModificado.NumCheque;
                cheque.FechaPago = chequeModificado.FechaPago;
                cheque.FechaEmision = chequeModificado.FechaEmision;
                cheque.Monto = chequeModificado.Monto;
                cheque.Proveedor = chequeModificado.Proveedor;
                cheque.Tipo = chequeModificado.Tipo;
                cheque.Estado = chequeModificado.Estado;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Eliminar un cheque
        public async Task<bool> DeleteCheque(int chequeId)
        {
            try
            {
                var cheque = await _context.Cheques.Where(c => c.ChequeId == chequeId).FirstOrDefaultAsync();
                
                if(cheque == null)
                {
                    return false;
                }
                _context.Cheques.Remove(cheque);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
