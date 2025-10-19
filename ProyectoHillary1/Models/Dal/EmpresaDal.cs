using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.En;

namespace ProyectoHillary1.Models.Dal
{
    public class EmpresaDal
    {

        private readonly ProyectoHillaryContext _context;

        // Constructor que recibe un objeto ProyectoHillaryContext para interactuar con la base de datos.
        public EmpresaDal(ProyectoHillaryContext proyectoHillaryContext)
        {
            _context = proyectoHillaryContext;
        }

        // Método para crear una nueva empresa en la base de datos.
        public async Task<int> Create(Empresa empresa)
        {
            // Asignar valores por defecto
            empresa.Activo = true;
            empresa.CreatedAt = DateTime.Now;
            empresa.UpdatedAt = DateTime.Now;

            _context.Add(empresa);
            return await _context.SaveChangesAsync();
        }

        // Método para obtener una empresa por su ID sin incluir relaciones.
        public async Task<Empresa> GetById(int id)
        {
            var empresa = await _context.empresa.FirstOrDefaultAsync(r => r.Id == id);
            return empresa ?? new Empresa();
        }

        // Método para obtener una empresa por su ID.
        //public async Task<Empresa> GetById(int id)
        //  {
        // var empresa = await _context.empresa
        //     .Include(e => e.Usuarios)
        //    .Include(e => e.Tareas)
        //     .FirstOrDefaultAsync(r => r.Id == id);
        //  return empresa ?? new Empresa();
        // }

        // Método para editar una empresa existente en la base de datos.
        public async Task<int> Edit(Empresa empresa)
        {
            int result = 0;
            var empresaUpdate = await GetById(empresa.Id);
            if (empresaUpdate.Id != 0)
            {
                // Actualiza los datos de la empresa.
                empresaUpdate.Nombre = empresa.Nombre;
                empresaUpdate.Ruc = empresa.Ruc;
                empresaUpdate.Direccion = empresa.Direccion;
                empresaUpdate.Telefono = empresa.Telefono;
                empresaUpdate.Email = empresa.Email;
                empresaUpdate.Activo = empresa.Activo ?? empresaUpdate.Activo;
                empresaUpdate.UpdatedAt = DateTime.Now;

                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método para eliminar un empresa de la base de datos por su ID.
        public async Task<int> Delete(int id)
        {
            int result = 0;
            var empresaDelete = await GetById(id);
            if (empresaDelete.Id > 0)
            {
                // Elimina el rol de la base de datos.
                _context.empresa.Remove(empresaDelete);
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método privado para construir una consulta IQueryable para buscar empresa con filtros.
        private IQueryable<Empresa> Query(Empresa empresa)
        {
            var query = _context.empresa.AsQueryable();

            if (!string.IsNullOrWhiteSpace(empresa.Nombre))
                query = query.Where(r => r.Nombre.Contains(empresa.Nombre));

            return query;
        }

        // Método para contar la cantidad de resultados de búsqueda con filtros.
        public async Task<int> CountSearch(Empresa empresa)
        {
            return await Query(empresa).CountAsync();
        }

        // Método para buscar empresa con filtros, paginación y ordenamiento.
        public async Task<List<Empresa>> Search(Empresa empresa, int take = 10, int skip = 0)
        {
            take = take == 0 ? 10 : take;
            var query = Query(empresa);
            query = query.OrderByDescending(r => r.Id).Skip(skip).Take(take);
            return await query.ToListAsync();
        }


        // Método para cambiar el estado activo/inactivo de una empresa.
        public async Task<int> ChangeStatus(int id, bool activo)
        {
            int result = 0;
            var empresa = await GetById(id);
            if (empresa.Id != 0)
            {
                empresa.Activo = activo;
                empresa.UpdatedAt = DateTime.Now;
                result = await _context.SaveChangesAsync();
            }
            return result;
        }
    }
}
