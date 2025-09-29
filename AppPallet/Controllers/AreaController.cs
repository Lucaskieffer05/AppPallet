using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    class AreaController
    {

        private readonly PalletContext _context;

        public AreaController(PalletContext context)
        {
            _context = context;
        }


        // Obtener todos los empresa
        public async Task<List<Area>> GetAllAreas()
        {
            try
            {
                return await _context.Area.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }



    }
}
