using AppPallet.Models;
using Microsoft.EntityFrameworkCore;
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
                nuevoCosto.Empresa = null!;
                nuevoCosto.Pallet = null!;

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

        // Obtener costo por pallet por id
        public async Task<CostoPorPallet?> GetCostoPorPalletById(int id)
        {
            try
            {
                return await _context.CostoPorPallets
                    .AsNoTracking()
                    .Include(CostoPorPallet => CostoPorPallet.CostoPorCamions)
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el costo por pallet: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateCostoPorPallet(CostoPorPallet costoPorPalletSeleccionada)
        {
            try
            {
                var costo = await _context.CostoPorPallets
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == costoPorPalletSeleccionada.CostoPorPalletId);

                if (costo == null)
                    return false;

                costo.NombrePalletCliente = costoPorPalletSeleccionada.NombrePalletCliente;
                costo.CantidadPorDia = costoPorPalletSeleccionada.CantidadPorDia;
                costo.CargaCamion = costoPorPalletSeleccionada.CargaCamion;
                costo.PalletId = costoPorPalletSeleccionada.PalletId;
                costo.EmpresaId = costoPorPalletSeleccionada.EmpresaId;
                costo.PrecioPallet = costoPorPalletSeleccionada.PrecioPallet;
                costo.GananciaPorCantPallet = costoPorPalletSeleccionada.GananciaPorCantPallet;
                costo.Mes = costoPorPalletSeleccionada.Mes;
                costo.HorasPorMes = costoPorPalletSeleccionada.HorasPorMes;

                // ❌ No poner null aquí
                // costo.CostoPorCamions = null!;

                var respuesta = await _context.SaveChangesAsync();
                if (respuesta <= 0)
                    return false;

                // Actualizar CostoPorCamions asociados
                var costosExistentes = await _context.CostoPorCamions
                    .Where(cc => cc.CostoPorPalletId == costo.CostoPorPalletId)
                    .ToListAsync();

                foreach (var costoExistente in costosExistentes)
                {
                    if (!costoPorPalletSeleccionada.CostoPorCamions
                        .Any(cc => cc.CostoPorCamionId == costoExistente.CostoPorCamionId))
                    {
                        _context.CostoPorCamions.Remove(costoExistente);
                    }
                }

                foreach (var costoCamion in costoPorPalletSeleccionada.CostoPorCamions)
                {
                    if (costoCamion.CostoPorCamionId == 0)
                    {
                        costoCamion.CostoPorPalletId = costo.CostoPorPalletId;
                        _context.CostoPorCamions.Add(costoCamion);
                    }
                    else
                    {
                        var existingCostoCamion = await _context.CostoPorCamions
                            .FirstOrDefaultAsync(cc => cc.CostoPorCamionId == costoCamion.CostoPorCamionId);
                        if (existingCostoCamion != null)
                        {
                            existingCostoCamion.NombreCosto = costoCamion.NombreCosto;
                            existingCostoCamion.Monto = costoCamion.Monto;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el costo por pallet: {ex.Message}");
                return false;
            }
        }

    }
}
