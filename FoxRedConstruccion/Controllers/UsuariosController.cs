// Controllers/UsuarioController.cs (Frontend MVC)
using FoxRedConstruccion.Services;
using Hillary.DTOs.UsuarioDTO;
using Microsoft.AspNetCore.Mvc;

namespace FoxRedConstruccion.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly EmpresaService _empresaService;

        public UsuariosController(UsuarioService usuarioService, EmpresaService empresaService)
        {
            _usuarioService = usuarioService;
            _empresaService = empresaService;
        }

        // GET: Usuario/Register?empresaId=5
        public async Task<IActionResult> Register(int? empresaId)
        {
            var model = new CreateUsuarioDTO();

            // Si viene un empresaId, verificar que existe y prellenar
            if (empresaId.HasValue && empresaId.Value > 0)
            {
                var empresa = await _empresaService.GetByIdAsync(empresaId.Value);

                if (empresa != null)
                {
                    model.EmpresaId = empresaId.Value;
                    ViewBag.EmpresaNombre = empresa.Nombre;
                    ViewBag.MostrarMensajeBienvenida = true;
                }
                else
                {
                    TempData["Warning"] = "La empresa especificada no existe";
                }
            }

            // RolId por defecto = 2 (Usuario normal)
            model.RolId = 2;

            return View(model);
        }

        // POST: Usuario/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"❌ Error de validación: {error.ErrorMessage}");
                }

                // Recargar información de la empresa para la vista
                if (usuario.EmpresaId > 0)
                {
                    var empresa = await _empresaService.GetByIdAsync(usuario.EmpresaId);
                    if (empresa != null)
                    {
                        ViewBag.EmpresaNombre = empresa.Nombre;
                    }
                }

                return View(usuario);
            }

            var (success, message, id, email, nombre) = await _usuarioService.RegisterAsync(usuario);

            if (success)
            {
                TempData["Success"] = $"¡Usuario '{nombre}' registrado exitosamente!";
                TempData["EmailGenerado"] = email;
                TempData["InfoMessage"] = "Se ha generado automáticamente un email para el usuario. Puede iniciar sesión con ese email y la contraseña que configuró.";

                // Redirigir al login o a donde desees
                return RedirectToAction("Login", "Auth");
            }

            TempData["Error"] = message ?? "No se pudo registrar el usuario. Intente nuevamente.";

            // Recargar información de la empresa
            if (usuario.EmpresaId > 0)
            {
                var empresa = await _empresaService.GetByIdAsync(usuario.EmpresaId);
                if (empresa != null)
                {
                    ViewBag.EmpresaNombre = empresa.Nombre;
                }
            }

            return View(usuario);
        }
    }
}