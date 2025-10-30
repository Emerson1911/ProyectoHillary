using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.En;

namespace ProyectoHillary1.Models.Dal
{
    public class TareaDal
    {
        private readonly ProyectoHillaryContext _context;

        // Constructor que recibe un objeto ProyectoHillaryContext para interactuar con la base de datos.
        public TareaDal(ProyectoHillaryContext proyectoHillaryContext)
        {
            _context = proyectoHillaryContext;
        }

        // Método para crear una nueva tarea en la base de datos.
        public async Task<int> Create(Tarea tarea)
        {
            _context.Add(tarea);
            return await _context.SaveChangesAsync();
        }

        // Método para obtener una tarea por su ID sin incluir relaciones.
        public async Task<Tarea> GetById(int id)
        {
            var tarea = await _context.tarea.FirstOrDefaultAsync(t => t.Id == id);
            return tarea ?? new Tarea();
        }

        // Método para obtener una tarea por su ID incluyendo la relación con Empresa.
        public async Task<Tarea> GetByIdWithEmpresa(int id)
        {
            var tarea = await _context.tarea
                .Include(t => t.Empresa)
                .FirstOrDefaultAsync(t => t.Id == id);
            return tarea ?? new Tarea();
        }

        // Método para editar una tarea existente en la base de datos.
        public async Task<int> Edit(Tarea tarea)
        {
            int result = 0;
            var tareaUpdate = await GetById(tarea.Id);
            if (tareaUpdate.Id != 0)
            {
                // Actualiza los datos de la tarea.
                tareaUpdate.EmpresaId = tarea.EmpresaId;
                tareaUpdate.Nombre = tarea.Nombre;
                tareaUpdate.Descripcion = tarea.Descripcion;

                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método para eliminar una tarea de la base de datos por su ID.
        public async Task<int> Delete(int id)
        {
            int result = 0;
            var tareaDelete = await GetById(id);
            if (tareaDelete.Id > 0)
            {
                // Elimina la tarea de la base de datos.
                _context.tarea.Remove(tareaDelete);
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        // Método privado para construir una consulta IQueryable para buscar tareas con filtros.
        private IQueryable<Tarea> Query(Tarea tarea)
        {
            var query = _context.tarea.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tarea.Nombre))
                query = query.Where(t => t.Nombre.Contains(tarea.Nombre));

            if (tarea.EmpresaId > 0)
                query = query.Where(t => t.EmpresaId == tarea.EmpresaId);

            return query;
        }

        // Método para contar la cantidad de resultados de búsqueda con filtros.
        public async Task<int> CountSearch(Tarea tarea)
        {
            return await Query(tarea).CountAsync();
        }

        // Método para buscar tareas con filtros, paginación y ordenamiento.
        public async Task<List<Tarea>> Search(Tarea tarea, int take = 10, int skip = 0)
        {
            take = take == 0 ? 10 : take;
            var query = Query(tarea);
            query = query.OrderByDescending(t => t.Id).Skip(skip).Take(take);
            return await query.ToListAsync();
        }

        // Método para buscar tareas incluyendo la relación con Empresa.
        public async Task<List<Tarea>> SearchWithEmpresa(Tarea tarea, int take = 10, int skip = 0)
        {
            take = take == 0 ? 10 : take;
            var query = Query(tarea);
            query = query.Include(t => t.Empresa)
                         .OrderByDescending(t => t.Id)
                         .Skip(skip)
                         .Take(take);
            return await query.ToListAsync();
        }

        // Método para obtener todas las tareas de una empresa específica incluyendo la relación con Empresa.
        public async Task<List<Tarea>> GetByEmpresaId(int empresaId)
        {
            return await _context.tarea
                .Include(t => t.Empresa) // Agrega esta línea
                .Where(t => t.EmpresaId == empresaId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }
    }
}