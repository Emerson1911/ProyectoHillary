using Hillary.DTOs.UsuarioDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoHillary1.Attributes;
using ProyectoHillary1.Controllers;
using ProyectoHillary1.Helpers;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Models.En;
using ProyectoHillary1.Services;

namespace ProyectoHillary1.Endpoints
{
    [Route("api/[controller]")]
    // ❌ NO poner [Authorize] aquí - cada endpoint lo maneja individualmente
    public class UsuarioController : BaseApiController
    {
        private readonly UsuarioDal _usuarioDal;
        private readonly JwtService _jwtService;

        public UsuarioController(UsuarioDal usuarioDal, JwtService jwtService)
        {
            _usuarioDal = usuarioDal;
            _jwtService = jwtService;
        }

        // POST: api/Usuario/login - NO requiere autenticación
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest(new { message = "Email y contraseña son requeridos" });
                }

                var usuario = await _usuarioDal.GetByEmailForLogin(loginDto.Email);

                if (usuario == null)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                bool isPasswordValid = PasswordHelper.VerifyPassword(loginDto.Password, usuario.Password!);

                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                string token = _jwtService.GenerateToken(usuario);
                DateTime expiresAt = _jwtService.GetTokenExpiration();

                var response = new LoginResponseDTO
                {
                    Token = token,
                    UserId = usuario.Id,
                    Nombre = usuario.Nombre ?? string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = usuario.Empresa?.Nombre ?? string.Empty,
                    RolId = usuario.RolId,
                    RolNombre = usuario.Rol?.Nombre ?? string.Empty,
                    ExpiresAt = expiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Usuario/me - Obtener info del usuario autenticado
        [AuthorizeRoles(1, 2)] // ✅ Gerentes y Usuarios pueden ver su propia info
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            return Ok(new
            {
                userId = GetUserId(),
                userName = GetUserName(),
                email = GetUserEmail(),
                empresaId = GetEmpresaId(),
                rolId = GetRolId(),
                rolNombre = GetRolNombre(),
                isAdmin = IsAdmin()
            });
        }

        // POST: api/Usuario - Crear usuario dentro de la empresa
        // ✅ Gerente (1) y Usuario (2) pueden crear usuarios
        // ✅ Usuario normal NO puede crear Gerentes (validación interna)
        [AuthorizeRoles(1, 2)]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUsuarioDTO createDto)
        {
            try
            {
                int empresaId = GetEmpresaId();
                int userRolId = GetRolId();

                if (empresaId == 0)
                {
                    return BadRequest(new { message = "No se pudo obtener la empresa del usuario autenticado" });
                }

                if (createDto.EmpresaId != 0 && createDto.EmpresaId != empresaId)
                {
                    return BadRequest(new { message = "Solo puedes crear usuarios para tu propia empresa" });
                }

                if (userRolId == 2 && createDto.RolId == 1)
                {
                    return BadRequest(new { message = "No tienes permisos para crear usuarios con rol Gerente" });
                }

                if (createDto.RolId == 0)
                {
                    return BadRequest(new { message = "Debe especificar un rol válido" });
                }

                var usuario = new Usuario
                {
                    EmpresaId = empresaId,
                    RolId = createDto.RolId,
                    Nombre = createDto.Nombre,
                    Password = PasswordHelper.HashPassword(createDto.Password),
                    Activo = true
                };

                int result = await _usuarioDal.Create(usuario);

                if (result > 0)
                {
                    return Ok(new
                    {
                        message = "Usuario creado exitosamente",
                        id = usuario.Id,
                        email = usuario.Email,
                        nombre = usuario.Nombre,
                        empresaId = usuario.EmpresaId
                    });
                }

                return BadRequest(new { message = "No se pudo crear el usuario" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear usuario: {ex.Message}");
                Console.WriteLine($"📍 StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Usuario/register - Registro público de usuarios
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] CreateUsuarioDTO createDto)
        {
            try
            {
                var usuario = new Usuario
                {
                    EmpresaId = createDto.EmpresaId,
                    RolId = createDto.RolId > 0 ? createDto.RolId : 1,
                    Nombre = createDto.Nombre,
                    Password = PasswordHelper.HashPassword(createDto.Password!),
                    Activo = true
                };

                int result = await _usuarioDal.Create(usuario);

                if (result > 0)
                {
                    return Ok(new
                    {
                        message = "Usuario registrado exitosamente",
                        id = usuario.Id,
                        email = usuario.Email,
                        nombre = usuario.Nombre
                    });
                }

                return BadRequest(new { message = "No se pudo registrar el usuario" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Usuario/{id}
        // ✅ Gerentes y Usuarios pueden ver usuarios de su empresa
        [AuthorizeRoles(1, 2)]
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

                // ✅ CORRECCIÓN: TODOS (admin o no) solo pueden ver usuarios de su misma empresa
                if (!BelongsToUserCompany(usuario.EmpresaId))
                {
                    return Forbid();
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
        // ✅ Gerentes pueden editar cualquier usuario de su empresa
        // ✅ Usuarios normales solo pueden editar su propio perfil (validación interna)
        [AuthorizeRoles(1, 2)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] EditUsuarioDTO editDto)
        {
            try
            {
                var usuarioExistente = await _usuarioDal.GetById(id);

                if (usuarioExistente.Id == 0)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // ✅ CORRECCIÓN: Solo puede editar usuarios de su misma empresa
                if (!BelongsToUserCompany(usuarioExistente.EmpresaId))
                {
                    return Forbid();
                }

                // ✅ Gerentes pueden editar cualquier usuario de su empresa
                // ✅ Usuarios normales solo pueden editar su propio perfil
                if (!IsAdmin() && GetUserId() != id)
                {
                    return Forbid();
                }

                var usuario = new Usuario
                {
                    Id = id,
                    Nombre = editDto.Nombre,
                    Email = editDto.Email,
                    Password = !string.IsNullOrWhiteSpace(editDto.Password)
                        ? PasswordHelper.HashPassword(editDto.Password)
                        : null,
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
        // ✅ SOLO Gerentes (RolId = 1) pueden eliminar usuarios
        [AuthorizeRoles(1)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var usuarioExistente = await _usuarioDal.GetById(id);

                if (usuarioExistente.Id == 0)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // ✅ Solo puede eliminar usuarios de su misma empresa
                if (!BelongsToUserCompany(usuarioExistente.EmpresaId))
                {
                    return Forbid();
                }

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
        // ✅ Gerentes y Usuarios pueden buscar usuarios de su empresa
        [AuthorizeRoles(1, 2)]
        [HttpPost("search")]
        public async Task<ActionResult<SearchResultUsuarioDTO>> Search([FromBody] SearchQueryUsuarioDTO searchDto)
        {
            try
            {
                Console.WriteLine($"🔍 Search recibido - Nombre: {searchDto.Nombre}, Email: {searchDto.Email}, RolId: {searchDto.RolId}, Activo: {searchDto.Activo}");
                Console.WriteLine($"👤 Usuario logueado - UserId: {GetUserId()}, EmpresaId: {GetEmpresaId()}, RolId: {GetRolId()}, IsAdmin: {IsAdmin()}");

                var usuario = new Usuario
                {
                    Nombre = searchDto.Nombre,
                    Email = searchDto.Email,
                    RolId = searchDto.RolId ?? 0,
                    Activo = searchDto.Activo
                };

                // ✅ CORRECCIÓN: SIEMPRE filtrar por la empresa del usuario logueado
                // No importa si es admin o no, solo ve usuarios de su misma empresa
                usuario.EmpresaId = GetEmpresaId();
                Console.WriteLine($"🏢 Filtrando por EmpresaId: {usuario.EmpresaId}");

                int skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
                int countRow = await _usuarioDal.CountSearch(usuario);
                var usuarios = await _usuarioDal.Search(usuario, searchDto.PageSize, skip);

                Console.WriteLine($"📊 Resultados encontrados: {countRow} usuarios de la empresa {usuario.EmpresaId}");

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
                Console.WriteLine($"❌ Error en Search: {ex.Message}");
                Console.WriteLine($"📍 StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PATCH: api/Usuario/{id}/change-status
        // ✅ SOLO Gerentes (RolId = 1) pueden cambiar estados
        [AuthorizeRoles(1)]
        [HttpPatch("{id}/change-status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] UserChangeStatusRequestDTO statusDto)
        {
            try
            {
                var usuarioExistente = await _usuarioDal.GetById(id);

                if (usuarioExistente.Id == 0)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // ✅ Solo puede cambiar estado de usuarios de su misma empresa
                if (!BelongsToUserCompany(usuarioExistente.EmpresaId))
                {
                    return Forbid();
                }

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