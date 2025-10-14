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

                _context.CostoPorPallet.Add(nuevoCosto);
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
                return await _context.CostoPorPallet
                    .AsNoTracking()
                    .Include(CostoPorPallet => CostoPorPallet.CostoPorCamion)
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el costo por pallet: {ex.Message}");
                return null;
            }
        }

        // Obtener los costos por pallet por empresa id
        public async Task<List<CostoPorPalletDTO>> GetCostosPorPalletByEmpresaId(int empresaId)
        {
            try
            {
                var fechaActual = DateTime.Now;
                var primerDiaMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var primerDiaMesAnterior = primerDiaMesActual.AddMonths(-1);

                var costos = await _context.CostoPorPallet
                    .AsNoTracking()
                    .Where(c => c.EmpresaId == empresaId &&
                                c.FechaDelete == null &&
                                c.Mes >= primerDiaMesAnterior &&
                                c.Mes < primerDiaMesActual.AddMonths(1))
                    .Include(c => c.Pallet)
                    .OrderByDescending(c => c.Mes)
                    .ToListAsync();

                // Mapeo manual de CostoPorPallet a CostoPorPalletDTO
                var costosDTO = costos.Select(c => new CostoPorPalletDTO
                {
                    CostoPorPalletId = c.CostoPorPalletId,
                    NombrePalletCliente = c.NombrePalletCliente,
                    FechaNombre = $"{c.Mes:MM/yyyy} - {c.NombrePalletCliente} - ${c.PrecioPallet}",
                    CantidadPorDia = c.CantidadPorDia,
                    CargaCamion = c.CargaCamion,
                    PalletId = c.PalletId,
                    Pallet = c.Pallet,
                    EmpresaId = c.EmpresaId,
                    PrecioPallet = c.PrecioPallet,
                    GananciaPorCantPallet = c.GananciaPorCantPallet,
                    Mes = c.Mes,
                    HorasPorMes = c.HorasPorMes
                    // Agrega aquí otros campos si tu DTO los requiere
                }).ToList();

                return costosDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los costos por pallet: {ex.Message}");
                return new List<CostoPorPalletDTO>();
            }
        }

        public async Task<bool> UpdateCostoPorPallet(CostoPorPallet costoPorPalletSeleccionada)
        {
            try
            {
                var costo = await _context.CostoPorPallet
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == costoPorPalletSeleccionada.CostoPorPalletId);

                if (costo == null)
                    return false;

                // Actualizar datos principales
                costo.NombrePalletCliente = costoPorPalletSeleccionada.NombrePalletCliente;
                costo.CantidadPorDia = costoPorPalletSeleccionada.CantidadPorDia;
                costo.CargaCamion = costoPorPalletSeleccionada.CargaCamion;
                costo.PalletId = costoPorPalletSeleccionada.PalletId;
                costo.EmpresaId = costoPorPalletSeleccionada.EmpresaId;
                costo.PrecioPallet = costoPorPalletSeleccionada.PrecioPallet;
                costo.GananciaPorCantPallet = costoPorPalletSeleccionada.GananciaPorCantPallet;
                costo.Mes = costoPorPalletSeleccionada.Mes;
                costo.HorasPorMes = costoPorPalletSeleccionada.HorasPorMes;

                // Actualizar CostoPorCamions asociados
                var costosExistentes = await _context.CostoPorCamion
                    .Where(cc => cc.CostoPorPalletId == costo.CostoPorPalletId)
                    .ToListAsync();

                // Eliminar los que ya no están
                foreach (var costoExistente in costosExistentes)
                {
                    if (!costoPorPalletSeleccionada.CostoPorCamion
                        .Any(cc => cc.CostoPorCamionId == costoExistente.CostoPorCamionId))
                    {
                        _context.CostoPorCamion.Remove(costoExistente);
                    }
                }

                // Agregar o actualizar los que vienen
                foreach (var costoCamion in costoPorPalletSeleccionada.CostoPorCamion)
                {
                    if (costoCamion.CostoPorCamionId == 0)
                    {
                        costoCamion.CostoPorPalletId = costo.CostoPorPalletId;
                        _context.CostoPorCamion.Add(costoCamion);
                    }
                    else
                    {
                        var existingCostoCamion = costosExistentes
                            .FirstOrDefault(cc => cc.CostoPorCamionId == costoCamion.CostoPorCamionId);

                        if (existingCostoCamion != null)
                        {
                            existingCostoCamion.NombreCosto = costoCamion.NombreCosto;
                            existingCostoCamion.Monto = costoCamion.Monto;
                        }
                    }
                }

                // ✅ Guardar todo en un solo SaveChangesAsync
                var respuesta = await _context.SaveChangesAsync();
                return respuesta > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el costo por pallet: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdatePrecioCostoPorPallet(CostoPorPallet costoPorPalletSeleccionada)
        {
            try
            {
                var costo = await _context.CostoPorPallet
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == costoPorPalletSeleccionada.CostoPorPalletId);

                if (costo == null)
                    return false;
                //No actualizar si los campos son los mimos
                if (costo.PrecioPallet == costoPorPalletSeleccionada.PrecioPallet &&
                    costo.GananciaPorCantPallet == costoPorPalletSeleccionada.GananciaPorCantPallet)
                {
                    return false; // No hay cambios, retornar true
                }

                // Actualizar solo los campos principales
                costo.PrecioPallet = costoPorPalletSeleccionada.PrecioPallet;
                costo.GananciaPorCantPallet = costoPorPalletSeleccionada.GananciaPorCantPallet;

                var respuesta = await _context.SaveChangesAsync();
                return respuesta > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el costo por pallet: {ex.Message}");
                return false;
            }
        }

        public async Task<MessageResult> DeleteCostoPorPallet(int costoPorPalletId)
        {
            try
            {
                var costo = await _context.CostoPorPallet
                    .Include(c => c.CostoPorCamion)
                    .FirstOrDefaultAsync(c => c.CostoPorPalletId == costoPorPalletId);
                if (costo == null)
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Presupuesto.NotFound);
                // Eliminar costos por camión asociados por fecha delete
                costo.FechaDelete = DateTime.Now;
                var respuesta = await _context.SaveChangesAsync();
                if (respuesta <= 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success,MessageConstants.Presupuesto.DeleteError);
                }
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Presupuesto.DeleteSuccess);

            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalle: {ex.Message}");
            }
        }
    }
}
