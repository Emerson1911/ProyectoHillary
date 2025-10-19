using Microsoft.AspNetCore.Mvc;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Models.En;
using Hillary.DTOs.RolDTOS;
using Microsoft.EntityFrameworkCore;

namespace ProyectoHillary1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly RolDal _rolDal;

        public RolController(RolDal rolDal)
        {
            _rolDal = rolDal;
        }

        // GET: api/Rol/search
        [HttpPost("search")]
        public async Task<ActionResult<SearchResultRolsDTO>> Search([FromBody] SearchQueryRolsDTO searchQuery)
        {
            var rol = new Rol
            {
                Nombre = searchQuery.Nombre
            };

            int skip = (searchQuery.PageNumber - 1) * searchQuery.PageSize;
            var roles = await _rolDal.Search(rol, searchQuery.PageSize, skip);
            int totalCount = await _rolDal.CountSearch(rol);

            var rolesDTO = roles.Select(r => new SearchResultRolsDTO.RolsDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                UsuariosCount = r.Usuarios?.Count ?? 0
            }).ToList();

            var result = new SearchResultRolsDTO
            {
                CountRow = totalCount,
                Data = rolesDTO
            };

            return Ok(result);
        }

        // GET: api/Rol/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetIdResultRolsDTO>> GetById(int id)
        {
            var rol = await _rolDal.GetById(id);

            if (rol.Id == 0)
                return NotFound(new { message = "Rol no encontrado" });

            var rolDTO = new GetIdResultRolsDTO
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                UsuariosCount = rol.Usuarios?.Count ?? 0
            };

            return Ok(rolDTO);
        }

        // POST: api/Rol/create
        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] CreateRolsDTO createRol)
        {
            var rol = new Rol
            {
                Nombre = createRol.Nombre,
                Descripcion = createRol.Descripcion
            };

            int result = await _rolDal.Create(rol);

            if (result > 0)
                return Created($"/api/rol/{rol.Id}", new { message = "Rol creado exitosamente" });

            return BadRequest(new { message = "Error al crear el rol" });
        }

        // PUT: api/Rol/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Edit(int id, [FromBody] EditRolsDTO editRol)
        {
            if (id != editRol.Id)
                return BadRequest(new { message = "El ID no coincide" });

            var rolExistente = await _rolDal.GetById(id);

            if (rolExistente.Id == 0)
                return NotFound(new { message = "Rol no encontrado" });

            var rol = new Rol
            {
                Id = editRol.Id,
                Nombre = editRol.Nombre,
                Descripcion = editRol.Descripcion
            };

            int result = await _rolDal.Edit(rol);

            if (result > 0)
                return Ok(new { message = "Rol actualizado exitosamente" });

            return BadRequest(new { message = "Error al actualizar el rol" });
        }

        // DELETE: api/Rol/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var rol = await _rolDal.GetById(id);

            if (rol.Id == 0)
                return NotFound(new { message = "Rol no encontrado" });

            int result = await _rolDal.Delete(id);

            if (result > 0)
                return Ok(new { message = "Rol eliminado exitosamente" });

            return BadRequest(new { message = "Error al eliminar el rol" });
        }
    }
}