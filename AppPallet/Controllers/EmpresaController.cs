using AppPallet.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<Empresa>> GetAllEmpresas()
        {
            try
            {
                return await _context.Empresas
                    .AsNoTracking()
                    .Include(e => e.ContactosEmpresas)
                        .ThenInclude(c => c.Area)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Crear un nuevo cliente/proveedor
        public async Task<bool> CreateEmpresa(Empresa nuevaEmpresa)
        {
            try
            {
                // 1. Agregar la empresa
                _context.Empresas.Add(nuevaEmpresa);
                await _context.SaveChangesAsync();

                // 2. Obtener las áreas fijas
                var areasFijas = await _context.Areas
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
                    _context.ContactosEmpresas.Add(contacto);
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

        // Modificar un cliente/proveedor existente
        public async Task<bool> UpdateEmpresa(Empresa EmpresaModificado)
        {
            try
            {
                var existingEntity = await _context.Empresas.FindAsync(EmpresaModificado.EmpresaId);
                if (existingEntity == null)
                {
                    Console.WriteLine("Cliente/Proveedor no encontrado.");
                    return false;
                }
                _context.Entry(existingEntity).CurrentValues.SetValues(EmpresaModificado);
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
        public async Task<bool> DeleteEmpresa(int EmpresaId)
        {
            try
            {
                var entity = await _context.Empresas.FindAsync(EmpresaId);
                if (entity == null)
                {
                    Console.WriteLine("Cliente/Proveedor no encontrado.");
                    return false;
                }
                _context.Empresas.Remove(entity);
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
    }
}
