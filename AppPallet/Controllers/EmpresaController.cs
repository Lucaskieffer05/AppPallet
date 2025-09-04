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
        public async Task<List<Empresa>> GetAllClientesProveedores()
        {
            try
            {
                return await _context.Empresas.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Crear un nuevo cliente/proveedor
        public async Task<bool> CreateEmpresa(Empresa nuevoEmpresa)
        {
            try
            {
                _context.Empresas.Add(nuevoEmpresa);
                var result = await _context.SaveChangesAsync();
                return result > 0; // SaveChangesAsync retorna el número de entradas afectadas
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
