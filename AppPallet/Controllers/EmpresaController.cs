using AppPallet.Constants;
using AppPallet.Models;
using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel.Chat;

namespace AppPallet.Controllers
{
    public class EmpresaController
    {
        private readonly PalletContext _context;

        public EmpresaController(PalletContext context)
        {
            _context = context;
        }
        // Métodos para manejar empresa (CRUD) pueden ser añadidos aquí

        // Obtener todos los empresa
        public async Task<List<Empresa>> GetAllEmpresas(string tipo)
        {
            try
            {
                return await _context.Empresa
                    .AsNoTracking()
                    .Where(c => c.Tipo == tipo && c.FechaDelete == null)
                    .Include(e => e.ContactosEmpresa)
                        .ThenInclude(c => c.Area)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public async Task<List<Empresa>> GetAllAloneEmpresas(string tipo = "Cliente")
        {
            try
            {
                return await _context.Empresa.AsNoTracking().Where(e => e.FechaDelete == null && e.Tipo == tipo).ToListAsync();
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<Empresa>> GetAllEmpresasCostoPallet(string tipo = "Cliente")
        {
            try
            {
                var mesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                return await _context.Empresa
                    .AsNoTracking()
                    .Where(e => e.FechaDelete == null && e.Tipo == tipo)
                    .Select(e => new Empresa
                    {
                        EmpresaId = e.EmpresaId,
                        NomEmpresa = e.NomEmpresa,
                        Cuit = e.Cuit,
                        Domicilio = e.Domicilio,
                        CostoPorPallet = e.CostoPorPallet
                            .Where(cp => cp.Mes.Year == mesActual.Year && cp.Mes.Month == mesActual.Month && cp.FechaDelete == null)
                            .ToList(),
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }


        // Obtener un empresa por ID
        public async Task<Empresa?> GetEmpresaById(int empresaId, DateTime filtroMes) {             
            try
            {
                return await _context.Empresa
                    .Include(e => e.CostoPorPallet
                        .Where(cp => cp.Mes.Year == filtroMes.Year && cp.Mes.Month == filtroMes.Month && cp.FechaDelete == null)
                        .OrderByDescending(cp => cp.Mes))
                    .ThenInclude(cp => cp.CostoPorCamion)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmpresaId == empresaId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Crear un nuevo cliente/proveedor
        public async Task<bool> CreateEmpresa(Empresa nuevaEmpresa)
        {
            try
            {
                // 1. Agregar la empresa
                _context.Empresa.Add(nuevaEmpresa);
                await _context.SaveChangesAsync();

                // 2. Obtener las áreas fijas
                var areasFijas = await _context.Area
                    .Where(a => a.NomArea == "VENTA" || a.NomArea == "ENTREGAS" || a.NomArea == "FACTURAS" || a.NomArea == "PAGOS")
                    .ToListAsync();

                // 3. Crear ContactosEmpresa vacíos para cada área
                foreach (var area in areasFijas)
                {
                    var contacto = new ContactosEmpresa
                    {
                        EmpresaId = nuevaEmpresa.EmpresaId,
                        AreaId = area.AreaId,
                        Contacto = null,
                        Mail = null,
                        Telefono = null,
                        Comentario = null,
                        Pallet = null,
                        Sello = null
                    };
                    _context.ContactosEmpresa.Add(contacto);
                }
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error de base de datos: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }

        // Modificar un empresa existente
        public async Task<bool> UpdateEmpresa(Empresa EmpresaModificado)
        {
            try
            {
                var existingEntity = await _context.Empresa
                    .Include(e => e.ContactosEmpresa)
                    .FirstOrDefaultAsync(e => e.EmpresaId == EmpresaModificado.EmpresaId);

                if (existingEntity == null)
                {
                    Console.WriteLine("Empresa no encontrado.");
                    return false;
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(EmpresaModificado);

                foreach (var contactoModificado in EmpresaModificado.ContactosEmpresa)
                {
                    var contactoExistente = existingEntity.ContactosEmpresa
                        .FirstOrDefault(c => c.ContactosEmpresaId == contactoModificado.ContactosEmpresaId);

                    if (contactoExistente != null)
                    {
                        _context.Entry(contactoExistente).CurrentValues.SetValues(contactoModificado);
                    }
                    else
                    {
                        existingEntity.ContactosEmpresa.Add(contactoModificado);
                    }
                }

                var result = await _context.SaveChangesAsync();
                return result > 0;

            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error de base de datos: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }

        // Eliminar un cliente/proveedor
        public async Task<MessageResult> DeleteEmpresa(int EmpresaId)
        {
            try
            {
                var entity = await _context.Empresa.FindAsync(EmpresaId);
                if (entity == null)
                {
                    return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Empresa.NotFound);
                }
                //Poner fechadelete en hoy
                entity.FechaDelete = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new MessageResult(MessageConstants.Titles.Success, MessageConstants.Empresa.Deleted);
                }
                return new MessageResult(MessageConstants.Titles.Error, MessageConstants.Empresa.NoDeleted);
            }
            catch (DbUpdateException dbEx)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Empresa.NoDeleted} Detalle: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return new MessageResult(MessageConstants.Titles.Error, $"{MessageConstants.Empresa.NoDeleted} Detalle: {ex.Message}");
            }
        }
    }
}
