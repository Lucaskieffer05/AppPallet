using AppPallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class CostoPorPalletController
    {
        private readonly PalletContext _context;

        public CostoPorPalletController(PalletContext context)
        {
            _context = context;
        }

        // crear un nuevo costo por pallet
        public async Task<bool> CreateCostoPorPallet(CostoPorPallet nuevoCosto)
        {
            try
            {
                nuevoCosto.Empresa = null;
                nuevoCosto.Pallet = null;
                nuevoCosto.Mes = null;

                _context.CostoPorPallets.Add(nuevoCosto);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    Console.WriteLine("Error al guardar el nuevo costo por pallet.");
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }

    }
}
