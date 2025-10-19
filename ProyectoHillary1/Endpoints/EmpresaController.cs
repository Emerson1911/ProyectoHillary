using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Models.En;
using Hillary.DTOs.EmpresaDTOS;

namespace ProyectoHillary1.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly EmpresaDal _empresaDal;

        public EmpresaController(EmpresaDal empresaDal)
        {
            _empresaDal = empresaDal;
        }

        // POST: api/Empresa
        [HttpPost]
        public async Task<ActionResult<GetIDResultEmpresaDTO>> Create([FromBody] CreateEmpresaDTO createDto)
        {
            try
            {
                // Mapear DTO a entidad
                var empresa = new Empresa
                {
                    Nombre = createDto.Nombre,
                    Ruc = createDto.Ruc,
                    Direccion = createDto.Direccion,
                    Telefono = createDto.Telefono,
                    Email = createDto.Email
                };

                // Crear en la base de datos
                var result = await _empresaDal.Create(empresa);

                if (result > 0)
                {
                    // Obtener la empresa creada
                    var empresaCreada = await _empresaDal.GetById(empresa.Id);

                    // Mapear a DTO de respuesta
                    var responseDto = new GetIDResultEmpresaDTO
                    {
                        Id = empresaCreada.Id,
                        Nombre = empresaCreada.Nombre,
                        Ruc = empresaCreada.Ruc,
                        Direccion = empresaCreada.Direccion,
                        Telefono = empresaCreada.Telefono,
                        Email = empresaCreada.Email,
                        Activo = empresaCreada.Activo,
                        CreatedAt = empresaCreada.CreatedAt,
                        UpdatedAt = empresaCreada.UpdatedAt,
                        TareasCount = 0,
                        UsuariosCount = 0
                    };

                    return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, responseDto);
                }

                return BadRequest(new { message = "No se pudo crear la empresa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Empresa/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetIDResultEmpresaDTO>> GetById(int id)
        {
            try
            {
                var empresa = await _empresaDal.GetById(id);

                if (empresa.Id == 0)
                    return NotFound(new { message = $"Empresa con ID {id} no encontrada" });

                // Mapear a DTO
                var responseDto = new GetIDResultEmpresaDTO
                {
                    Id = empresa.Id,
                    Nombre = empresa.Nombre,
                    Ruc = empresa.Ruc,
                    Direccion = empresa.Direccion,
                    Telefono = empresa.Telefono,
                    Email = empresa.Email,
                    Activo = empresa.Activo,
                    CreatedAt = empresa.CreatedAt,
                    UpdatedAt = empresa.UpdatedAt,
                    TareasCount = empresa.Tareas?.Count ?? 0,
                    UsuariosCount = empresa.Usuarios?.Count ?? 0
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Empresa/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<GetIDResultEmpresaDTO>> Edit(int id, [FromBody] EditEmpresaDTO editDto)
        {
            try
            {
                // Validar que existe
                var empresaExistente = await _empresaDal.GetById(id);
                if (empresaExistente.Id == 0)
                    return NotFound(new { message = $"Empresa con ID {id} no encontrada" });

                // Mapear DTO a entidad
                var empresa = new Empresa
                {
                    Id = id,
                    Nombre = editDto.Nombre,
                    Ruc = editDto.Ruc,
                    Direccion = editDto.Direccion,
                    Telefono = editDto.Telefono,
                    Email = editDto.Email,
                    Activo = editDto.Activo
                };

                // Actualizar
                var result = await _empresaDal.Edit(empresa);

                if (result > 0)
                {
                    // Obtener empresa actualizada
                    var empresaActualizada = await _empresaDal.GetById(id);

                    var responseDto = new GetIDResultEmpresaDTO
                    {
                        Id = empresaActualizada.Id,
                        Nombre = empresaActualizada.Nombre,
                        Ruc = empresaActualizada.Ruc,
                        Direccion = empresaActualizada.Direccion,
                        Telefono = empresaActualizada.Telefono,
                        Email = empresaActualizada.Email,
                        Activo = empresaActualizada.Activo,
                        CreatedAt = empresaActualizada.CreatedAt,
                        UpdatedAt = empresaActualizada.UpdatedAt,
                        TareasCount = empresaActualizada.Tareas?.Count ?? 0,
                        UsuariosCount = empresaActualizada.Usuarios?.Count ?? 0
                    };

                    return Ok(responseDto);
                }

                return BadRequest(new { message = "No se pudo actualizar la empresa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Empresa/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var empresa = await _empresaDal.GetById(id);
                if (empresa.Id == 0)
                    return NotFound(new { message = $"Empresa con ID {id} no encontrada" });

                var result = await _empresaDal.Delete(id);

                if (result > 0)
                    return Ok(new { message = "Empresa eliminada exitosamente" });

                return BadRequest(new { message = "No se pudo eliminar la empresa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Empresa/search
        [HttpPost("search")]
        public async Task<ActionResult<SearchResultEmpresaDTO>> Search([FromBody] SearchQueryEmpresaDTO searchQuery)
        {
            try
            {
                // Crear entidad para búsqueda
                var empresaBusqueda = new Empresa
                {
                    Nombre = searchQuery.Nombre,
                    Ruc = searchQuery.Ruc,
                    Email = searchQuery.Email,
                    Activo = searchQuery.Activo
                };

                // Calcular skip
                int skip = (searchQuery.PageNumber - 1) * searchQuery.PageSize;

                // Obtener total de registros
                var totalCount = await _empresaDal.CountSearch(empresaBusqueda);

                // Obtener empresas paginadas
                var empresas = await _empresaDal.Search(empresaBusqueda, searchQuery.PageSize, skip);

                // Mapear a DTO
                var empresasDto = empresas.Select(e => new SearchResultEmpresaDTO.EmpresaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Ruc = e.Ruc,
                    Direccion = e.Direccion,
                    Telefono = e.Telefono,
                    Email = e.Email,
                    Activo = e.Activo,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    TareasCount = e.Tareas?.Count ?? 0,
                    UsuariosCount = e.Usuarios?.Count ?? 0
                }).ToList();

                var result = new SearchResultEmpresaDTO
                {
                    CountRow = totalCount,
                    Data = empresasDto
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PATCH: api/Empresa/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] ChangeStatusRequestDTO request)
        {
            try
            {
                var empresa = await _empresaDal.GetById(id);
                if (empresa.Id == 0)
                    return NotFound(new { message = $"Empresa con ID {id} no encontrada" });

                var result = await _empresaDal.ChangeStatus(id, request.Activo);

                if (result > 0)
                {
                    var mensaje = request.Activo ? "activada" : "desactivada";
                    return Ok(new { message = $"Empresa {mensaje} exitosamente" });
                }

                return BadRequest(new { message = "No se pudo cambiar el estado de la empresa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}