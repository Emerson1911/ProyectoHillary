// Controllers/EmpresaController.cs
using FoxRedConstruccion.Services;
using Hillary.DTOs.EmpresaDTOS;
using Microsoft.AspNetCore.Mvc;

namespace FoxRedConstruccion.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly EmpresaService _empresaService;

        public EmpresaController(EmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        // GET: Empresa/Index
        public async Task<IActionResult> Index(string? nombre, string? ruc, bool? activo, int pageNumber = 1, int pageSize = 10)
        {
            var searchQuery = new SearchQueryEmpresaDTO
            {
                Nombre = nombre,
                Ruc = ruc,
                Activo = activo,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _empresaService.SearchAsync(searchQuery);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result?.CountRow ?? 0;
            ViewBag.TotalPages = (int)Math.Ceiling((result?.CountRow ?? 0) / (double)pageSize);
            ViewBag.Nombre = nombre;
            ViewBag.Ruc = ruc;
            ViewBag.Activo = activo;

            return View(result);
        }

        // GET: Empresa/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);

            if (empresa == null)
            {
                TempData["Error"] = "Empresa no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(empresa);
        }

        // GET: Empresa/Create
        // GET: Empresa/Create
        public IActionResult Create()
        {
            // ✅ IMPORTANTE: Pasar un modelo nuevo (no null) para que los Tag Helpers funcionen
            var model = new CreateEmpresaDTO();
            return View(model);
        }

        // POST: Empresa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmpresaDTO empresa)
        {
            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, mostrarlos
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error de validación: {error.ErrorMessage}");
                }
                return View(empresa);
            }

            var result = await _empresaService.CreateAsync(empresa);

            if (result != null)
            {
                TempData["Success"] = "Empresa creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudo crear la empresa";
            return View(empresa);
        }

        // GET: Empresa/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);

            if (empresa == null)
            {
                TempData["Error"] = "Empresa no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var editDto = new EditEmpresaDTO
            {
                Nombre = empresa.Nombre,
                Ruc = empresa.Ruc,
                Direccion = empresa.Direccion,
                Telefono = empresa.Telefono,
                Email = empresa.Email,
                Activo = empresa.Activo
            };

            ViewBag.Id = id;
            return View(editDto);
        }

        // POST: Empresa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEmpresaDTO empresa)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(empresa);
            }

            var result = await _empresaService.EditAsync(id, empresa);

            if (result != null)
            {
                TempData["Success"] = "Empresa actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudo actualizar la empresa";
            ViewBag.Id = id;
            return View(empresa);
        }

        // GET: Empresa/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);

            if (empresa == null)
            {
                TempData["Error"] = "Empresa no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(empresa);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _empresaService.DeleteAsync(id);

            if (result)
            {
                TempData["Success"] = "Empresa eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la empresa";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Empresa/ChangeStatus/5
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, bool activo)
        {
            var result = await _empresaService.ChangeStatusAsync(id, activo);

            if (result)
            {
                return Json(new { success = true, message = $"Empresa {(activo ? "activada" : "desactivada")} exitosamente" });
            }

            return Json(new { success = false, message = "No se pudo cambiar el estado" });
        }
    }
}