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

        // GET: Usuarios/Index - Lista de usuarios con búsqueda
        public async Task<IActionResult> Index(string? nombre, string? email, int? rolId, bool? activo, int pageNumber = 1, int pageSize = 9)
        {
            try
            {
                var searchDto = new SearchQueryUsuarioDTO
                {
                    Nombre = nombre,
                    Email = email,
                    RolId = rolId,
                    Activo = activo,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _usuarioService.SearchAsync(searchDto);

                if (result != null)
                {
                    // Pasar filtros a la vista para mantenerlos en el formulario
                    ViewBag.Nombre = nombre;
                    ViewBag.Email = email;
                    ViewBag.RolId = rolId;
                    ViewBag.Activo = activo;
                    ViewBag.CurrentPage = pageNumber;
                    ViewBag.TotalCount = result.CountRow;
                    ViewBag.TotalPages = (int)Math.Ceiling((double)result.CountRow / pageSize);

                    return View(result);
                }

                return View(new SearchResultUsuarioDTO());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Index: {ex.Message}");
                TempData["Error"] = "Error al cargar la lista de usuarios";
                return View(new SearchResultUsuarioDTO());
            }
        }

        // GET: Usuarios/Create - Mostrar formulario de creación (interno)
        public IActionResult Create()
        {
            var model = new CreateUsuarioDTO
            {
                RolId = 2 // Valor por defecto: Usuario estándar
            };
            return View(model);
        }

        // POST: Usuarios/Create - Crear usuario (interno)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"❌ Error de validación: {error.ErrorMessage}");
                }
                return View(usuario);
            }

            try
            {
                var result = await _usuarioService.CreateAsync(usuario);

                if (result.Success)
                {
                    TempData["Success"] = $"Usuario '{result.Nombre}' creado exitosamente";
                    TempData["InfoMessage"] = $"Email generado: {result.Email}";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message ?? "No se pudo crear el usuario";
                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear usuario: {ex.Message}");
                TempData["Error"] = "Error interno al crear el usuario";
                return View(usuario);
            }
        }

        // GET: Usuarios/Register - Registro público
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

        // POST: Usuarios/Register - Registro público
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

        // GET: Usuarios/Details/5 - Ver detalles
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Details: {ex.Message}");
                TempData["Error"] = "Error al cargar los detalles del usuario";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Usuarios/Edit/5 - Mostrar formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var editDto = new EditUsuarioDTO
                {
                    Nombre = usuario.Nombre,
                    Email = usuario.Email
                };

                ViewBag.Id = id;
                return View(editDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Edit GET: {ex.Message}");
                TempData["Error"] = "Error al cargar el usuario para editar";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuarios/Edit/5 - Actualizar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUsuarioDTO editDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(editDto);
            }

            try
            {
                var result = await _usuarioService.EditAsync(id, editDto);

                if (result)
                {
                    TempData["Success"] = "Usuario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No se pudo actualizar el usuario";
                ViewBag.Id = id;
                return View(editDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Edit POST: {ex.Message}");
                TempData["Error"] = "Error al actualizar el usuario";
                ViewBag.Id = id;
                return View(editDto);
            }
        }

        // GET: Usuarios/Delete/5 - Mostrar confirmación de eliminación
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Delete GET: {ex.Message}");
                TempData["Error"] = "Error al cargar el usuario para eliminar";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuarios/Delete/5 - Eliminar usuario
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _usuarioService.DeleteAsync(id);

                if (result)
                {
                    TempData["Success"] = "Usuario eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el usuario";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en DeleteConfirmed: {ex.Message}");
                TempData["Error"] = "Error al eliminar el usuario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuarios/ChangeStatus - Cambiar estado activo/inactivo
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, bool activo)
        {
            try
            {
                var result = await _usuarioService.ChangeStatusAsync(id, activo);

                if (result)
                {
                    return Json(new { success = true, message = "Estado actualizado exitosamente" });
                }

                return Json(new { success = false, message = "No se pudo actualizar el estado" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ChangeStatus: {ex.Message}");
                return Json(new { success = false, message = "Error al cambiar el estado" });
            }
        }
    }
}