using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatriculasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Matriculas
        public async Task<IActionResult> Index()
        {
            var matriculas = _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .OrderByDescending(m => m.FechaMatricula);

            return View(await matriculas.ToListAsync());
        }

        // GET: Matriculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.MatriculaId == id);

            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // GET: Matriculas/Create
        public IActionResult Create()
        {
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes.OrderBy(e => e.Nombre), "EstudianteId", "Nombre");
            ViewData["CursoId"] = new SelectList(_context.Cursos.OrderBy(c => c.Nombre), "CursoId", "Nombre");
            return View();
        }

        // POST: Matriculas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatriculaId,EstudianteId,CursoId,FechaMatricula,Estado")] Matricula matricula)
        {
            bool existe = await _context.Matriculas
                .AnyAsync(m => m.EstudianteId == matricula.EstudianteId && m.CursoId == matricula.CursoId);

            if (existe)
            {
                ModelState.AddModelError(string.Empty, "El estudiante ya está matriculado en este curso.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(matricula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes.OrderBy(e => e.Nombre), "EstudianteId", "Nombre", matricula.EstudianteId);
            ViewData["CursoId"] = new SelectList(_context.Cursos.OrderBy(c => c.Nombre), "CursoId", "Nombre", matricula.CursoId);
            return View(matricula);
        }

        // GET: Matriculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.MatriculaId == id);

            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // POST: Matriculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula != null)
            {
                _context.Matriculas.Remove(matricula);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}