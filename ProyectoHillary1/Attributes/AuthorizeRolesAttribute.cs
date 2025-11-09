using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProyectoHillary1.Attributes
{
    /// <summary>
    /// Atributo para restringir acceso por roles específicos
    /// Uso: [AuthorizeRoles(1, 2)] permite solo roles 1 y 2
    /// </summary>
    public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public AuthorizeRolesAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Verificar si está autenticado
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "No autenticado. Por favor inicie sesión."
                });
                return;
            }

            // Obtener el RolId del token
            var rolIdClaim = user.FindFirst("RolId")?.Value;

            if (string.IsNullOrEmpty(rolIdClaim) || !int.TryParse(rolIdClaim, out int rolId))
            {
                context.Result = new ObjectResult(new
                {
                    message = "No se pudo obtener el rol del usuario."
                })
                {
                    StatusCode = 403
                };
                return;
            }

            // Verificar si el rol está permitido
            if (!_allowedRoles.Contains(rolId))
            {
                context.Result = new ObjectResult(new
                {
                    message = "No tiene permisos para realizar esta acción.",
                    requiredRoles = _allowedRoles,
                    yourRole = rolId
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
}