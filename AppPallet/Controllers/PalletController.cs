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
                return await _context.Pallet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        //Sumar stock
        public async Task<MessageResult> SumarStockPallet(int palletId, int cantidad)
        {
            try
            {
                var pallet = await _context.Pallet.FindAsync(palletId);
                if (pallet == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pallet.NotFound);
                }
                pallet.Stock += cantidad;
                if (pallet.Stock < 0)
                    pallet.Stock = 0;
                await _context.SaveChangesAsync();
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Pallet.ModifySuccess);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Pallet.ModifyError} Detalles: {ex.Message}");
            }
        }


    }
}
