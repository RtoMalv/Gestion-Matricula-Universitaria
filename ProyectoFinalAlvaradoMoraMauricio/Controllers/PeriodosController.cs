using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PeriodosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeriodosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.PeriodosAcademicos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.NombrePeriodo.Contains(search));
            }

            var periodos = await query
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(periodos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var periodo = await _context.PeriodosAcademicos
                .FirstOrDefaultAsync(p => p.PeriodoAcademicoId == id);

            if (periodo == null) return NotFound();

            return View(periodo);
        }

        public IActionResult Create()
        {
            return View(new PeriodoAcademicoViewModel
            {
                FechaInicio = DateTime.Today,
                FechaFinal = DateTime.Today.AddMonths(4),
                Activo = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PeriodoAcademicoViewModel model)
        {
            if (model.FechaFinal < model.FechaInicio)
            {
                ModelState.AddModelError("FechaFinal", "La fecha final no puede ser menor que la fecha de inicio.");
            }

            if (ModelState.IsValid)
            {
                var periodo = new PeriodoAcademico
                {
                    NombrePeriodo = model.NombrePeriodo,
                    FechaInicio = model.FechaInicio,
                    FechaFinal = model.FechaFinal,
                    Activo = model.Activo
                };

                _context.PeriodosAcademicos.Add(periodo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var periodo = await _context.PeriodosAcademicos.FindAsync(id);
            if (periodo == null) return NotFound();

            var model = new PeriodoAcademicoViewModel
            {
                PeriodoAcademicoId = periodo.PeriodoAcademicoId,
                NombrePeriodo = periodo.NombrePeriodo,
                FechaInicio = periodo.FechaInicio,
                FechaFinal = periodo.FechaFinal,
                Activo = periodo.Activo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PeriodoAcademicoViewModel model)
        {
            if (id != model.PeriodoAcademicoId) return NotFound();

            if (model.FechaFinal < model.FechaInicio)
            {
                ModelState.AddModelError("FechaFinal", "La fecha final no puede ser menor que la fecha de inicio.");
            }

            if (ModelState.IsValid)
            {
                var periodo = await _context.PeriodosAcademicos.FindAsync(id);
                if (periodo == null) return NotFound();

                periodo.NombrePeriodo = model.NombrePeriodo;
                periodo.FechaInicio = model.FechaInicio;
                periodo.FechaFinal = model.FechaFinal;
                periodo.Activo = model.Activo;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var periodo = await _context.PeriodosAcademicos
                .FirstOrDefaultAsync(p => p.PeriodoAcademicoId == id);

            if (periodo == null) return NotFound();

            return View(periodo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var periodo = await _context.PeriodosAcademicos.FindAsync(id);
            if (periodo == null) return NotFound();

            _context.PeriodosAcademicos.Remove(periodo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}