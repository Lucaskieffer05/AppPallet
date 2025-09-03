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
        private readonly PalletContex _context;

        public ChequeController(PalletContex context)
        {
            _context = context;
        }

        // Obtener todos los cheques
        public async Task<List<Cheque>> GetAllCheques()
        {
            try
            {
                return await _context.Cheques.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Modificar un cheque existente
        public async Task<bool> UpdateCheque(Cheque cheque)
        {
            try
            {
                _context.Cheques.Update(cheque);
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
