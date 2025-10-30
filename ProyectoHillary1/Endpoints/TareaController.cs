using Hillary.DTOs.TareaDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoHillary1.Controllers;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Models.En;

namespace ProyectoHillary1.Endpoints
{
    [Route("api/[controller]")]
    [Authorize] // ✅ AGREGAR aquí
    public class TareaController : BaseApiController
    {
        private readonly TareaDal _tareaDal;

        public TareaController(TareaDal tareaDal)
        {
            _tareaDal = tareaDal;
        }

        // POST: api/Tarea
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateTareaDTO createDto)
        {
            try
            {
                int empresaId = GetEmpresaId();

                if (empresaId == 0)
                    return BadRequest(new { message = "No se pudo obtener la empresa del usuario" });

                var tarea = new Tarea
                {
                    EmpresaId = empresaId,
                    Nombre = createDto.Nombre,
                    Descripcion = createDto.Descripcion
                };

                int result = await _tareaDal.Create(tarea);

                if (result > 0)
                    return Ok(new { message = "Tarea creada exitosamente", id = tarea.Id });

                return BadRequest(new { message = "No se pudo crear la tarea" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Tarea/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetIDResultTareaDTO>> GetById(int id)
        {
            try
            {
                var tarea = await _tareaDal.GetByIdWithEmpresa(id);

                if (tarea.Id == 0)
                    return NotFound(new { message = "Tarea no encontrada" });

                // Verificar que la tarea pertenece a la empresa del usuario
                if (!BelongsToUserCompany(tarea.EmpresaId))
                    return Forbid();

                var result = new GetIDResultTareaDTO
                {
                    Id = tarea.Id,
                    EmpresaId = tarea.EmpresaId,
                    Nombre = tarea.Nombre,
                    Descripcion = tarea.Descripcion,
                    EmpresaNombre = tarea.Empresa?.Nombre
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Tarea/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] EditTareaDTO editDto)
        {
            try
            {
                int empresaId = GetEmpresaId();

                var tareaExistente = await _tareaDal.GetByIdWithEmpresa(id);
                if (tareaExistente.Id == 0)
                    return NotFound(new { message = "Tarea no encontrada" });

                if (!BelongsToUserCompany(tareaExistente.EmpresaId))
                    return Forbid();

                var tarea = new Tarea
                {
                    Id = id,
                    EmpresaId = empresaId,
                    Nombre = editDto.Nombre,
                    Descripcion = editDto.Descripcion
                };

                int result = await _tareaDal.Edit(tarea);

                if (result > 0)
                    return Ok(new { message = "Tarea actualizada exitosamente" });

                return NotFound(new { message = "Tarea no encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Tarea/{id} - Solo admin
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (!IsAdmin())
                    return Forbid();

                var tarea = await _tareaDal.GetByIdWithEmpresa(id);
                if (tarea.Id == 0)
                    return NotFound(new { message = "Tarea no encontrada" });

                if (!BelongsToUserCompany(tarea.EmpresaId))
                    return Forbid();

                int result = await _tareaDal.Delete(id);

                if (result > 0)
                    return Ok(new { message = "Tarea eliminada exitosamente" });

                return NotFound(new { message = "Tarea no encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Tarea/search
        [HttpPost("search")]
        public async Task<ActionResult<SearchResultTareaDTO>> Search([FromBody] SearchQueryTareaDTO searchQuery)
        {
            try
            {
                int empresaId = GetEmpresaId();

                if (empresaId == 0)
                    return BadRequest(new { message = "No se pudo obtener la empresa del usuario" });

                var tarea = new Tarea
                {
                    Nombre = searchQuery.Nombre,
                    EmpresaId = empresaId
                };

                int skip = (searchQuery.PageNumber - 1) * searchQuery.PageSize;
                int take = searchQuery.PageSize;

                var tareas = await _tareaDal.SearchWithEmpresa(tarea, take, skip);
                int totalCount = await _tareaDal.CountSearch(tarea);

                var result = new SearchResultTareaDTO
                {
                    CountRow = totalCount,
                    Data = tareas.Select(t => new SearchResultTareaDTO.TareaDTO
                    {
                        Id = t.Id,
                        EmpresaId = t.EmpresaId,
                        Nombre = t.Nombre,
                        Descripcion = t.Descripcion,
                        EmpresaNombre = t.Empresa?.Nombre
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Tarea/mis-tareas
        [HttpGet("mis-tareas")]
        public async Task<ActionResult<List<SearchResultTareaDTO.TareaDTO>>> GetMisTareas()
        {
            try
            {
                int empresaId = GetEmpresaId();

                if (empresaId == 0)
                    return BadRequest(new { message = "No se pudo obtener la empresa del usuario" });

                var tareas = await _tareaDal.GetByEmpresaId(empresaId);

                var result = tareas.Select(t => new SearchResultTareaDTO.TareaDTO
                {
                    Id = t.Id,
                    EmpresaId = t.EmpresaId,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    EmpresaNombre = t.Empresa?.Nombre
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        
    }
}