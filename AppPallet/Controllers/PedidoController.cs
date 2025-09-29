using AppPallet.Constants;
using AppPallet.Models;
using AppPallet.Views;
using Microsoft.EntityFrameworkCore;

namespace AppPallet.Controllers
{
    public class PedidoController
    {
        private readonly PalletContext _context;

        public PedidoController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los pedidos
        public async Task<List<Pedido>> GetAllPedidos()
        {
            try
            {
                return await _context.Pedido
                    .AsNoTracking()
                    .OrderByDescending(p => p.FechaEInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Pedido>();
            }
        }

        public async Task<List<Pedido>> GetPedidosByLoteId(int loteId)
        {
            try
            {
                return await _context.Pedido
                    .AsNoTracking()
                    .Include(p => p.Pallet)
                    .Where(p => p.LoteId == loteId)
                    .OrderByDescending(p => p.FechaEInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Pedido>();
            }
        }


        // Crear un nuevo pedido
        public async Task<MessageResult> CreatePedido(Pedido nuevoPedido)
        {
            try
            {
                Pedido pedido = new Pedido
                {
                    PalletId = nuevoPedido.PalletId,
                    FechaEInicio = nuevoPedido.FechaEInicio,
                    Cantidad = nuevoPedido.Cantidad,
                    LoteId = nuevoPedido.LoteId,
                    FechaEFinal = nuevoPedido.FechaEFinal
                };
                _context.Pedido.Add(pedido);
                var result  = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.CreateError);
                }
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Pedido.CreateSuccess);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.CreateError + $"{ex.Message}");
            }
        }

        // Actualizar un pedido existente
        public async Task<MessageResult> UpdatePedido(Pedido pedidoActualizado)
        {
            try
            {
                var pedidoExistente = await _context.Pedido.FindAsync(pedidoActualizado.PedidoId);
                if (pedidoExistente == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.NotFound);
                }
                pedidoExistente.PalletId = pedidoActualizado.PalletId;
                pedidoExistente.FechaEInicio = pedidoActualizado.FechaEInicio;
                pedidoExistente.Cantidad = pedidoActualizado.Cantidad;
                pedidoExistente.LoteId = pedidoActualizado.LoteId;
                pedidoExistente.FechaEFinal = pedidoActualizado.FechaEFinal;
                _context.Pedido.Update(pedidoExistente);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.ModifyNoChanges);
                }
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Pedido.ModifySuccess);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Generic.UnexpectedError + $"{ex.Message}");
            }
        }

        // Eliminar un pedido
        public async Task<MessageResult> DeletePedido(int pedidoId)
        {
            try
            {
                var pedidoExistente = await _context.Pedido.FindAsync(pedidoId);
                if (pedidoExistente == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.NotFound);
                }
                _context.Pedido.Remove(pedidoExistente);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Pedido.DeleteError);
                }
                return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Pedido.DeleteSuccess);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Generic.UnexpectedError + $"{ex.Message}");
            }
        }

    }
}
