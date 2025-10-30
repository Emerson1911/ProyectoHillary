using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProyectoHillary1.Controllers
{
    /// <summary>
    /// Controlador base con métodos helper para JWT
    /// Todos tus controllers deben heredar de esta clase
    /// </summary>
    [ApiController]
    // ❌ NO poner [Authorize] aquí - cada controller lo maneja individualmente
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado desde el token JWT
        /// </summary>
        protected int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return 0;
            }
            return userId;
        }

        /// <summary>
        /// Obtiene el EmpresaId del usuario autenticado desde el token JWT
        /// </summary>
        protected int GetEmpresaId()
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (string.IsNullOrEmpty(empresaIdClaim) || !int.TryParse(empresaIdClaim, out int empresaId))
            {
                return 0;
            }
            return empresaId;
        }

        /// <summary>
        /// Obtiene el RolId del usuario autenticado desde el token JWT
        /// </summary>
        protected int GetRolId()
        {
            var rolIdClaim = User.FindFirst("RolId")?.Value;
            if (string.IsNullOrEmpty(rolIdClaim) || !int.TryParse(rolIdClaim, out int rolId))
            {
                return 0;
            }
            return rolId;
        }

        /// <summary>
        /// Obtiene el nombre del usuario autenticado desde el token JWT
        /// </summary>
        protected string GetUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtiene el email del usuario autenticado desde el token JWT
        /// </summary>
        protected string GetUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtiene el nombre del rol del usuario autenticado desde el token JWT
        /// </summary>
        protected string GetRolNombre()
        {
            return User.FindFirst("RolNombre")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Verifica si el usuario tiene un rol específico
        /// </summary>
        protected bool HasRole(int rolId)
        {
            return GetRolId() == rolId;
        }

        /// <summary>
        /// Verifica si el usuario tiene alguno de los roles especificados
        /// </summary>
        protected bool HasAnyRole(params int[] rolesIds)
        {
            int userRolId = GetRolId();
            return rolesIds.Contains(userRolId);
        }

        /// <summary>
        /// Verifica si el usuario es administrador (RolId = 1)
        /// </summary>
        protected bool IsAdmin()
        {
            return GetRolId() == 1;
        }

        /// <summary>
        /// Verifica si un recurso pertenece a la empresa del usuario autenticado
        /// </summary>
        protected bool BelongsToUserCompany(int resourceEmpresaId)
        {
            return GetEmpresaId() == resourceEmpresaId;
        }
    }
}