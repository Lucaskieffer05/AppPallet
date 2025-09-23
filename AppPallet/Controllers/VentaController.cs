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
    public class VentaController
    {

        private readonly PalletContext _context;

        public VentaController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todas las ventas
        public async Task<List<Venta>> GetAllVentas()
        {
            try
            {
                return await _context.Venta
                    .AsNoTracking()
                    .Include(v => v.CostoPorPallet)
                    .ThenInclude(cp => cp.Empresa)
                    .Include(v => v.CostoPorPallet)
                    .ThenInclude(cp => cp.Pallet)
                    .OrderBy(v => v.Estado)
                    .ThenBy(v => v.FechaVenta)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Venta>();
            }
        }

        // Obtener venta por ID
        public async Task<Venta?> GetVentaById(int id)
        {
            try
            {
                return await _context.Venta
                    .AsNoTracking()
                    .Include(v => v.CostoPorPallet)
                    .ThenInclude(cp => cp.Empresa)
                    .Include(v => v.CostoPorPallet)
                    .ThenInclude(cp => cp.Pallet)
                    .FirstOrDefaultAsync(v => v.VentaId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Crear una nueva venta
        public async Task<MessageResult> CreateVenta(Venta nuevaVenta)
        {
            try
            {
                // Crear una nueva instancia
                Venta Venta = new()
                {
                    FechaVenta = nuevaVenta.FechaVenta,
                    CantPallets = nuevaVenta.CantPallets,
                    Estado = nuevaVenta.Estado,
                    CostoPorPalletId = nuevaVenta.CostoPorPalletId,
                    Comentario = nuevaVenta.Comentario,
                    FechaEntrega = nuevaVenta.FechaEntrega
                };

                _context.Venta.Add(nuevaVenta);
                var respuesta = await _context.SaveChangesAsync();
                if (respuesta > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Venta.CreateSuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Venta.CreateError);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Generic.UnexpectedError);
            }
        }

        // Modificar una venta existente
        public async Task<MessageResult> UpdateVenta(Venta ventaActualizada)
        {
            try
            {
                var ventaExistente = await _context.Venta.FindAsync(ventaActualizada.VentaId);
                if (ventaExistente == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Venta.NotFound);
                }
                //Ver si hay algun cambio
                bool hayCambios = ventaExistente.FechaVenta != ventaActualizada.FechaVenta ||
                                 ventaExistente.CantPallets != ventaActualizada.CantPallets ||
                                 ventaExistente.Estado != ventaActualizada.Estado ||
                                 ventaExistente.CostoPorPalletId != ventaActualizada.CostoPorPalletId ||
                                 ventaExistente.Comentario != ventaActualizada.Comentario ||
                                 ventaExistente.FechaEntrega != ventaActualizada.FechaEntrega;

                if (!hayCambios)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Venta.ModifyNoChanges);
                }


                // Actualizar los campos
                ventaExistente.FechaVenta = ventaActualizada.FechaVenta;
                ventaExistente.CantPallets = ventaActualizada.CantPallets;
                ventaExistente.Estado = ventaActualizada.Estado;
                ventaExistente.CostoPorPalletId = ventaActualizada.CostoPorPalletId;
                ventaExistente.Comentario = ventaActualizada.Comentario;
                ventaExistente.FechaEntrega = ventaActualizada.FechaEntrega;
                var respuesta = await _context.SaveChangesAsync();
                if (respuesta > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Venta.ModifySuccess);
                }
                else
                {
                    return new MessageResult(MessageConstants.Titles.Warning, MessageConstants.Venta.ModifyNoChanges);
                }
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Venta.ModifyError);
            }
        }

        // Eliminar una venta
        public async Task<MessageResult> DeleteVenta(int ventaId)
        {
            try
            {
                var ventaExistente = await _context.Venta.FindAsync(ventaId);
                if (ventaExistente == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Venta.NotFound);
                }
                _context.Venta.Remove(ventaExistente);
                var respuesta = await _context.SaveChangesAsync();
                if (respuesta > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Venta.DeleteSuccess);
                }
                else
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Venta.DeleteError);
                }
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Generic.UnexpectedError + $" {ex.Message}");
            }
        }

    }
}
