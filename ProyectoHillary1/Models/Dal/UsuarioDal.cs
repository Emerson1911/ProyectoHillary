using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.En;
using ProyectoHillary1.Utilities;

namespace ProyectoHillary1.Models.Dal
{
    public class UsuarioDal
    {
        private readonly ProyectoHillaryContext _context;
        private readonly EmailGenerator _emailGenerator;  // ← AGREGAR ESTA LÍNEA

        // Constructor actualizado para inyectar EmailGenerator
        public UsuarioDal(ProyectoHillaryContext proyectoHillaryContext, EmailGenerator emailGenerator)
        {
            _context = proyectoHillaryContext;
            _emailGenerator = emailGenerator;  // ← AGREGAR ESTA LÍNEA
        }

        public async Task<int> Create(Usuario usuario)
        {
            // Autogenerar email si no se proporciona
            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                usuario.Email = await _emailGenerator.GenerarEmailUnico(usuario.Nombre, usuario.EmpresaId);
            }

            usuario.Activo = true;
            _context.Add(usuario);
            return await _context.SaveChangesAsync();
        }

        // Método para obtener una usuario por su ID CON relaciones de Empresa y Rol.
        public async Task<Usuario> GetById(int id)
        {
            var usuario = await _context.usuario
                .Include(u => u.Empresa)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(r => r.Id == id);
            return usuario ?? new Usuario();
        }

        // Método para editar una usuario existente en la base de datos.
        public async Task<int> Edit(Usuario usuario)
        {
            int result = 0;
            var usuarioUpdate = await GetById(usuario.Id);
            if (usuarioUpdate.Id != 0)
            {
                // Actualiza los datos de la usuario.
                usuarioUpdate.Nombre = usuario.Nombre;

                // Solo actualizar si no es null o vacío
                if (!string.IsNullOrWhiteSpace(usuario.Email))
                    usuarioUpdate.Email = usuario.Email;

                if (!string.IsNullOrWhiteSpace(usuario.Password))
                    usuarioUpdate.Password = usuario.Password;

                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método para eliminar un usuario de la base de datos por su ID.
        public async Task<int> Delete(int id)
        {
            int result = 0;
            var usuarioDelete = await GetById(id);
            if (usuarioDelete.Id > 0)
            {
                // Elimina el usuario de la base de datos.
                _context.usuario.Remove(usuarioDelete);
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método privado para construir una consulta IQueryable para buscar usuario con filtros.
        private IQueryable<Usuario> Query(Usuario usuario)
        {
            var query = _context.usuario.AsQueryable();
            if (!string.IsNullOrWhiteSpace(usuario.Nombre))
                query = query.Where(r => r.Nombre.Contains(usuario.Nombre));
            if (!string.IsNullOrWhiteSpace(usuario.Email))
                query = query.Where(r => r.Email.Contains(usuario.Email));
            // Filtrar por EmpresaId cuando venga especificado (> 0)
            if (usuario.EmpresaId > 0)
                query = query.Where(r => r.EmpresaId == usuario.EmpresaId);
            return query;
        }

        // Método para contar la cantidad de resultados de búsqueda con filtros.
        public async Task<int> CountSearch(Usuario usuario)
        {
            return await Query(usuario).CountAsync();
        }

        // Método para buscar usuario con filtros, paginación y ordenamiento CON relaciones.
        public async Task<List<Usuario>> Search(Usuario usuario, int take = 10, int skip = 0)
        {
            take = take == 0 ? 10 : take;
            var query = Query(usuario);
            query = query
                .Include(u => u.Empresa)
                .Include(u => u.Rol)
                .OrderByDescending(r => r.Id)
                .Skip(skip)
                .Take(take);
            return await query.ToListAsync();
        }

        // Método para cambiar el estado activo/inactivo de un usuario.
        public async Task<int> ChangeStatus(int id, bool activo)
        {
            int result = 0;
            var usuario = await GetById(id);
            if (usuario.Id != 0)
            {
                usuario.Activo = activo;
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método para obtener usuario por email (para login)
        public async Task<Usuario?> GetByEmailForLogin(string email)
        {
            var usuario = await _context.usuario
                .Include(u => u.Empresa)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo == true);

            return usuario;
        }
    }
}