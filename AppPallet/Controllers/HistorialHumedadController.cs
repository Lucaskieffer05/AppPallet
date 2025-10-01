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
    public class HistorialHumedadController
    {

        private readonly PalletContext _context;

        public HistorialHumedadController(PalletContext context)
        {
            _context = context;
        }

        //Obtener el historial por pedido
        public async Task<List<HistorialHumedad>> GetHistorialHumedadPedidoId(int PedidoId)
        {
            try
            {
                return await _context.HistorialHumedad.AsNoTracking().Where(h => h.PedidoId == PedidoId).ToListAsync();

            }
            catch {
                return new List<HistorialHumedad>();
            }
        }

        // crear historial
        public async Task<MessageResult> CreateHistorialHumedad(HistorialHumedad newHistorial)
        {
            try
            {
                HistorialHumedad historial = new HistorialHumedad
                {
                    PedidoId = newHistorial.PedidoId,
                    Fecha = newHistorial.Fecha,
                    Min = newHistorial.Min,
                    Max = newHistorial.Max,
                    Promedio = newHistorial.Promedio,
                    PesoAprox = newHistorial.PesoAprox,
                };

                _context.HistorialHumedad.Add(historial);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.HistorialHumedad.CreateSuccess);
                }

                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.HistorialHumedad.CreateError);

            }
            catch (Exception ex)
            {

                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} - {ex.Message}");
            }
        }

        //editar historial
        public async Task<MessageResult> EditHistorialHumedad(HistorialHumedad editHistorial)
        {
            try
            {
                var historial = await _context.HistorialHumedad.FindAsync(editHistorial.HistorialHumedadId);
                if (historial == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.HistorialHumedad.NotFound);
                }
                historial.Fecha = editHistorial.Fecha;
                historial.Min = editHistorial.Min;
                historial.Max = editHistorial.Max;
                historial.Promedio = editHistorial.Promedio;
                historial.PesoAprox = editHistorial.PesoAprox;
                _context.HistorialHumedad.Update(historial);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.HistorialHumedad.ModifySuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.HistorialHumedad.ModifyNoChanges);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} - {ex.Message}");
            }
        }

        // eliminar historial
        public async Task<MessageResult> DeleteHistorialHumedad(int historialId)
        {
            try
            {
                var historial = await _context.HistorialHumedad.FindAsync(historialId);
                if (historial == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.HistorialHumedad.NotFound);
                }
                _context.HistorialHumedad.Remove(historial);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.HistorialHumedad.DeleteSuccess);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.HistorialHumedad.DeleteError);
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Generic.UnexpectedError} - {ex.Message}");
            }
        }


    }
}
