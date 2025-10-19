using Hillary.DTOs.UsuarioDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoHillary1.Helpers;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Models.En;

namespace ProyectoHillary1.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioDal _usuarioDal;

        public UsuarioController(UsuarioDal usuarioDal)
        {
            _usuarioDal = usuarioDal;
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUsuarioDTO createDto)
        {
            try
            {
                var usuario = new Usuario
                {
                    EmpresaId = createDto.EmpresaId,
                    RolId = createDto.RolId,
                    Nombre = createDto.Nombre,
                    //Email = createDto.Email,
                   Password = PasswordHelper.HashPassword(createDto.Password), // HASH AQUÍ
                   // Activo = createDto.Activo ?? true
                };

                int result = await _usuarioDal.Create(usuario);

                if (result > 0)
                {
                    return Ok(new { message = "Usuario creado exitosamente", id = usuario.Id });
                }

                return BadRequest(new { message = "No se pudo crear el usuario" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Usuario/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetlResultUsuarioDTO>> GetById(int id)
        {
            try
            {
                var usuario = await _usuarioDal.GetById(id);

                if (usuario.Id == 0)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                var result = new GetlResultUsuarioDTO
                {
                    Id = usuario.Id,
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = usuario.Empresa?.Nombre,
                    RolId = usuario.RolId,
                    RolNombre = usuario.Rol?.Nombre,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Activo = usuario.Activo
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Usuario/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] EditUsuarioDTO editDto)
        {
            try
            {
                var usuario = new Usuario
                {
                    Id = id,
                    Nombre = editDto.Nombre,
                    Email = editDto.Email,
                    // Solo hashear si se proporciona una nueva contraseña
                    Password = !string.IsNullOrWhiteSpace(editDto.Password)
                        ? PasswordHelper.HashPassword(editDto.Password)
                        : null,
                    //Activo = editDto.Activo
                };

                int result = await _usuarioDal.Edit(usuario);

                if (result > 0)
                {
                    return Ok(new { message = "Usuario actualizado exitosamente" });
                }

                return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Usuario/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                int result = await _usuarioDal.Delete(id);

                if (result > 0)
                {
                    return Ok(new { message = "Usuario eliminado exitosamente" });
                }

                return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Usuario/search
        [HttpPost("search")]
        public async Task<ActionResult<SearchResultUsuarioDTO>> Search([FromBody] SearchQueryUsuarioDTO searchDto)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nombre = searchDto.Nombre,
                    Email = searchDto.Email
                };

                int skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
                int countRow = await _usuarioDal.CountSearch(usuario);
                var usuarios = await _usuarioDal.Search(usuario, searchDto.PageSize, skip);

                var result = new SearchResultUsuarioDTO
                {
                    CountRow = countRow,
                    Data = usuarios.Select(u => new SearchResultUsuarioDTO.UsuarioDTO
                    {
                        Id = u.Id,
                        EmpresaId = u.EmpresaId,
                        EmpresaNombre = u.Empresa?.Nombre,
                        RolId = u.RolId,
                        RolNombre = u.Rol?.Nombre,
                        Nombre = u.Nombre,
                        Email = u.Email,
                        Activo = u.Activo
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PATCH: api/Usuario/{id}/change-status
        [HttpPatch("{id}/change-status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] UserChangeStatusRequestDTO statusDto)
        {
            try
            {
                int result = await _usuarioDal.ChangeStatus(id, statusDto.Activo);

                if (result > 0)
                {
                    return Ok(new { message = "Estado actualizado exitosamente" });
                }

                return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}