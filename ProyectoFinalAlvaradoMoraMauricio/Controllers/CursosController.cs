using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Cursos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(search) ||
                    c.Codigo.Contains(search));
            }

            var cursos = await query
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(cursos);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // GET: Cursos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CursoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeCodigo = await _context.Cursos
                    .AnyAsync(c => c.Codigo == model.Codigo);

                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Ya existe un curso con ese código.");
                    return View(model);
                }

                var curso = new Curso
                {
                    Codigo = model.Codigo,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Creditos = model.Creditos,
                    Activo = model.Activo
                };

                _context.Cursos.Add(curso);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            var model = new CursoViewModel
            {
                CursoId = curso.CursoId,
                Codigo = curso.Codigo,
                Nombre = curso.Nombre,
                Descripcion = curso.Descripcion,
                Creditos = curso.Creditos,
                Activo = curso.Activo
            };

            return View(model);
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CursoViewModel model)
        {
            if (id != model.CursoId) return NotFound();

            if (ModelState.IsValid)
            {
                var curso = await _context.Cursos.FindAsync(id);
                if (curso == null) return NotFound();

                var existeCodigo = await _context.Cursos
                    .AnyAsync(c => c.Codigo == model.Codigo && c.CursoId != model.CursoId);

                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Ya existe otro curso con ese código.");
                    return View(model);
                }

                curso.Codigo = model.Codigo;
                curso.Nombre = model.Nombre;
                curso.Descripcion = model.Descripcion;
                curso.Creditos = model.Creditos;
                curso.Activo = model.Activo;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Cursos/Requisitos/5
        public async Task<IActionResult> Requisitos(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos
                .Include(c => c.RequisitosDelCurso)
                    .ThenInclude(r => r.CursoRequisitoNavigation)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            var cursosDisponibles = await _context.Cursos
                .Where(c => c.Activo && c.CursoId != curso.CursoId)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CursoBaseId = curso.CursoId;
            ViewBag.CursoBaseCodigo = curso.Codigo;
            ViewBag.CursoBaseNombre = curso.Nombre;
            ViewBag.RequisitosActuales = curso.RequisitosDelCurso
                .OrderBy(r => r.TipoRequisito)
                .ThenBy(r => r.CursoRequisitoNavigation.Nombre)
                .ToList();

            ViewBag.CursoRequisitoIdFk = new SelectList(cursosDisponibles, "CursoId", "NombreMostrar");

            var model = new CursoRequisitoViewModel
            {
                CursoId = curso.CursoId,
                TipoRequisito = "Requisito"
            };

            return View(model);
        }

        // POST: Cursos/Requisitos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Requisitos(CursoRequisitoViewModel model)
        {
            var curso = await _context.Cursos
                .Include(c => c.RequisitosDelCurso)
                    .ThenInclude(r => r.CursoRequisitoNavigation)
                .FirstOrDefaultAsync(c => c.CursoId == model.CursoId);

            if (curso == null) return NotFound();

            if (model.CursoId == model.CursoRequisitoIdFk)
            {
                ModelState.AddModelError(string.Empty, "Un curso no puede ser requisito de sí mismo.");
            }

            var yaExiste = await _context.CursoRequisitos.AnyAsync(r =>
                r.CursoId == model.CursoId &&
                r.CursoRequisitoIdFk == model.CursoRequisitoIdFk &&
                r.TipoRequisito == model.TipoRequisito);

            if (yaExiste)
            {
                ModelState.AddModelError(string.Empty, "Esa relación ya existe para el curso seleccionado.");
            }

            if (ModelState.IsValid)
            {
                var requisito = new CursoRequisito
                {
                    CursoId = model.CursoId,
                    CursoRequisitoIdFk = model.CursoRequisitoIdFk,
                    TipoRequisito = model.TipoRequisito
                };

                _context.CursoRequisitos.Add(requisito);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Requisitos), new { id = model.CursoId });
            }

            var cursosDisponibles = await _context.Cursos
                .Where(c => c.Activo && c.CursoId != curso.CursoId)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CursoBaseId = curso.CursoId;
            ViewBag.CursoBaseCodigo = curso.Codigo;
            ViewBag.CursoBaseNombre = curso.Nombre;
            ViewBag.RequisitosActuales = curso.RequisitosDelCurso
                .OrderBy(r => r.TipoRequisito)
                .ThenBy(r => r.CursoRequisitoNavigation.Nombre)
                .ToList();

            ViewBag.CursoRequisitoIdFk = new SelectList(cursosDisponibles, "CursoId", "NombreMostrar", model.CursoRequisitoIdFk);

            return View(model);
        }
    }
}