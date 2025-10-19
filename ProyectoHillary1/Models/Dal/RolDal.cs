using ProyectoHillary1.Models.En;
using Microsoft.EntityFrameworkCore;

namespace ProyectoHillary1.Models.Dal
{
    public class RolDal
    {
        private readonly ProyectoHillaryContext _context;

        // Constructor que recibe un objeto ProyectoHillaryContext para interactuar con la base de datos.
        public RolDal(ProyectoHillaryContext proyectoHillaryContext)
        {
            _context = proyectoHillaryContext;
        }

        // Método para crear un nuevo rol en la base de datos.
        public async Task<int> Create(Rol rol)
        {
            _context.Add(rol);
            return await _context.SaveChangesAsync();
        }

        // Método para obtener un rol por su ID.
        public async Task<Rol> GetById(int id)
        {
            var rol = await _context.rol.FirstOrDefaultAsync(r => r.Id == id);
            return rol ?? new Rol();
        }

        // Método para editar un rol existente en la base de datos.
        public async Task<int> Edit(Rol rol)
        {
            int result = 0;
            var rolUpdate = await GetById(rol.Id);
            if (rolUpdate.Id != 0)
            {
                // Actualiza los datos del rol.
                rolUpdate.Nombre = rol.Nombre;
                rolUpdate.Descripcion = rol.Descripcion;

                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método para eliminar un rol de la base de datos por su ID.
        public async Task<int> Delete(int id)
        {
            int result = 0;
            var rolDelete = await GetById(id);
            if (rolDelete.Id > 0)
            {
                // Elimina el rol de la base de datos.
                _context.rol.Remove(rolDelete);
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método privado para construir una consulta IQueryable para buscar roles con filtros.
        private IQueryable<Rol> Query(Rol rol)
        {
            var query = _context.rol.AsQueryable();

            if (!string.IsNullOrWhiteSpace(rol.Nombre))
                query = query.Where(r => r.Nombre.Contains(rol.Nombre));

            return query;
        }

        // Método para contar la cantidad de resultados de búsqueda con filtros.
        public async Task<int> CountSearch(Rol rol)
        {
            return await Query(rol).CountAsync();
        }

        // Método para buscar roles con filtros, paginación y ordenamiento.
        public async Task<List<Rol>> Search(Rol rol, int take = 10, int skip = 0)
        {
            take = take == 0 ? 10 : take;
            var query = Query(rol);
            query = query.OrderByDescending(r => r.Id).Skip(skip).Take(take);
            return await query.ToListAsync();
        }
    }
}