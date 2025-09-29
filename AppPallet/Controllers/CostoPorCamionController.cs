using AppPallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Controllers
{
    public class CostoPorCamionController
    {

        private readonly PalletContext _context;

        public CostoPorCamionController(PalletContext context)
        {
            _context = context;
        }

        // Eliminar
        public async Task<bool> DeleteCostoPorCamion(int costoPorCamionId)
        {
            try
            {
                var costoPorCamion = await _context.CostoPorCamion.FindAsync(costoPorCamionId);
                if (costoPorCamion == null)
                {
                    return false;
                }
                _context.CostoPorCamion.Remove(costoPorCamion);
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    Console.WriteLine("Error al eliminar el costo por camión.");
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
