// Controllers/TareaController.cs
using FoxRedConstruccion.Services;
using Hillary.DTOs.TareaDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoxRedConstruccion.Controllers
{
    [Authorize]
    public class TareaController : Controller
    {
        private readonly TareaService _tareaService;

        public TareaController(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        // GET: Tarea/Index
        public async Task<IActionResult> Index(string? nombre, int pageNumber = 1, int pageSize = 10)
        {
            var searchQuery = new SearchQueryTareaDTO
            {
                Nombre = nombre,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _tareaService.SearchAsync(searchQuery);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result?.CountRow ?? 0;
            ViewBag.TotalPages = (int)Math.Ceiling((result?.CountRow ?? 0) / (double)pageSize);
            ViewBag.Nombre = nombre;

            return View(result);
        }

        // GET: Tarea/MisTareas
        public async Task<IActionResult> MisTareas()
        {
            var tareas = await _tareaService.GetMisTareasAsync();
            return View(tareas);
        }

        // GET: Tarea/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tarea = await _tareaService.GetByIdAsync(id);

            if (tarea == null)
            {
                TempData["Error"] = "Tarea no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(tarea);
        }

        // GET: Tarea/Create
        public IActionResult Create()
        {
            var model = new CreateTareaDTO();
            return View(model);
        }

        // POST: Tarea/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTareaDTO tarea)
        {
            if (!ModelState.IsValid)
            {
                return View(tarea);
            }

            var result = await _tareaService.CreateAsync(tarea);

            if (result)
            {
                TempData["Success"] = $"Tarea '{tarea.Nombre}' creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudo crear la tarea";
            return View(tarea);
        }

        // GET: Tarea/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tarea = await _tareaService.GetByIdAsync(id);

            if (tarea == null)
            {
                TempData["Error"] = "Tarea no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var editDto = new EditTareaDTO
            {
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion
            };

            ViewBag.Id = id;
            return View(editDto);
        }

        // POST: Tarea/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTareaDTO tarea)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(tarea);
            }

            var result = await _tareaService.EditAsync(id, tarea);

            if (result)
            {
                TempData["Success"] = "Tarea actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudo actualizar la tarea";
            ViewBag.Id = id;
            return View(tarea);
        }

        // GET: Tarea/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _tareaService.GetByIdAsync(id);

            if (tarea == null)
            {
                TempData["Error"] = "Tarea no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(tarea);
        }

        // POST: Tarea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _tareaService.DeleteAsync(id);

            if (result)
            {
                TempData["Success"] = "Tarea eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la tarea";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}