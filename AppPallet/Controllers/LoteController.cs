using AppPallet.Constants;
using AppPallet.Models;
using Microsoft.EntityFrameworkCore;

namespace AppPallet.Controllers
{
    public class LoteController
    {

        private readonly PalletContext _context;

        public LoteController(PalletContext context)
        {
            _context = context;
        }

        // Obtener todos los lotes
        public async Task<List<Lote>> GetAllLotes()
        {
            try
            {
                return await _context.Lote
                    .AsNoTracking()
                    .OrderByDescending(l => l.FechaSolicitada)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Lote>();
            }
        }

        public async Task<List<LoteMostrarDTO>> GetAllLotesMostrar(DateTime filtroMes)
        {
            try
            {
                var lotes = await _context.Lote
                    .AsNoTracking()
                    .Include(l => l.Empresa)
                    .Include(l => l.Pedido)
                    .ThenInclude(p => p.Pallet)
                    .Where(l => l.FechaSolicitada.Year == filtroMes.Year && l.FechaSolicitada.Month == filtroMes.Month)
                    .OrderByDescending(l => l.FechaSolicitada)
                    .ToListAsync();

                return lotes.Select(l => new LoteMostrarDTO
                {
                    LoteId = l.LoteId,
                    NumLote = l.NumLote,
                    FechaSolicitada = l.FechaSolicitada,
                    FechaEntrega = l.FechaEntrega,
                    NomProveedor = l.Empresa.NomEmpresa,
                    Empresa = l.Empresa,
                    EmpresaId = l.EmpresaId,
                    PrecioProveedor = l.PrecioProveedor,
                    NumFacturaProveedor = l.NumFacturaProveedor,
                    NomCamionero = l.NomCamionero,
                    PrecioCamionero = l.PrecioCamionero,
                    Pedido = l.Pedido,
                    TotalPedidos = l.Pedido.Count,
                    TotalPallets = l.Pedido.Sum(p => p.Cantidad),
                    PedidosDetallados = l.Pedido.ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<LoteMostrarDTO>();
            }
        }

        // Obtener lote por ID
        public async Task<Lote?> GetLoteById(int id)
        {
            try
            {
                return await _context.Lote
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LoteId == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Crear un nuevo lote
        public async Task<MessageResult> CreateLote(Lote nuevoLote)
        {
            try
            {
                Lote newLote = new Lote
                {
                    NumLote = nuevoLote.NumLote,
                    FechaSolicitada = nuevoLote.FechaSolicitada,
                    FechaEntrega = nuevoLote.FechaEntrega,
                    EmpresaId = nuevoLote.EmpresaId,
                    PrecioProveedor = nuevoLote.PrecioProveedor,
                    NumFacturaProveedor = nuevoLote.NumFacturaProveedor,
                    NomCamionero = nuevoLote.NomCamionero,
                    PrecioCamionero = nuevoLote.PrecioCamionero,
                };

                _context.Lote.Add(nuevoLote);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    nuevoLote.NumLote = nuevoLote.LoteId;

                    await _context.SaveChangesAsync();

                    return new MessageResult(MessageConstants.Titles.Success,
                        MessageConstants.Lote.CreateSuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Lote.CreateError);
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {ex.Message}");
            }
        }

        // Modificar un lote existente
        public async Task<MessageResult> UpdateLote(Lote loteModificado)
        {
            try
            {
                var lote = await _context.Lote.Where(l => l.LoteId == loteModificado.LoteId).FirstOrDefaultAsync();
                if (lote == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Lote.NotFound);
                }
                // Verificar si hay cambios
                if (lote.NumLote == loteModificado.NumLote &&
                    lote.FechaSolicitada == loteModificado.FechaSolicitada &&
                    lote.FechaEntrega == loteModificado.FechaEntrega &&
                    lote.EmpresaId == loteModificado.EmpresaId &&
                    lote.PrecioProveedor == loteModificado.PrecioProveedor &&
                    lote.NumFacturaProveedor == loteModificado.NumFacturaProveedor &&
                    lote.NomCamionero == loteModificado.NomCamionero &&
                    lote.PrecioCamionero == loteModificado.PrecioCamionero)
                {
                    return new MessageResult(MessageConstants.Titles.Warning, MessageConstants.Lote.ModifyNoChanges);
                }
                // Actualizar los campos
                lote.NumLote = loteModificado.NumLote;
                lote.FechaSolicitada = loteModificado.FechaSolicitada;
                lote.FechaEntrega = loteModificado.FechaEntrega;
                lote.EmpresaId = loteModificado.EmpresaId;
                lote.PrecioProveedor = loteModificado.PrecioProveedor;
                lote.NumFacturaProveedor = loteModificado.NumFacturaProveedor;
                lote.NomCamionero = loteModificado.NomCamionero;
                lote.PrecioCamionero = loteModificado.PrecioCamionero;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Lote.ModifySuccess);
                }
                else
                {
                    return new MessageResult(MessageConstants.Titles.Warning, MessageConstants.Lote.ModifyNoChanges);
                }
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {ex.Message}");

            }
        }

        // Eliminar un lote
        public async Task<MessageResult> DeleteLote(int id)
        {
            try
            {
                var lote = await _context.Lote.Where(l => l.LoteId == id).FirstOrDefaultAsync();
                if (lote == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Lote.NotFound);
                }
                // Verificar si el lote tiene pedidos asociados
                var tienePedidos = await _context.Pedido.AnyAsync(p => p.LoteId == id);
                if (tienePedidos)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Lote.DeleteHasPedidos);
                }


                _context.Lote.Remove(lote);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Lote.DeleteSuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Lote.DeleteError);
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} Detalles: {ex.Message}");
            }
        }
    }
}
