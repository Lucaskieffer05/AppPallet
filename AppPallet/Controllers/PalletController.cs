using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class PalletController
    {
        private readonly PalletContext _context;

        public PalletController(PalletContext context)
        {
            _context = context;
        }

        // Métodos para manejar pallet (CRUD) pueden ser añadidos aquí

        // get all
        public async Task<List<Pallet>> GetAllPallets()
        {
            try
            {
                return await _context.Pallets.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

    }
}
