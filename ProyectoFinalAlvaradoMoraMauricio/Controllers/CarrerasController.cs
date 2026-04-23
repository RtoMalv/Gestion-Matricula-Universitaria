using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CarrerasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrerasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carreras
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Carreras.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(search) ||
                    c.Codigo.Contains(search));
            }

            var carreras = await query
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(carreras);
        }

        // GET: Carreras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null) return NotFound();

            return View(carrera);
        }

        // GET: Carreras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carreras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarreraViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeCodigo = await _context.Carreras
                    .AnyAsync(c => c.Codigo == model.Codigo);

                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Ya existe una carrera con ese código.");
                    return View(model);
                }

                var carrera = new Carrera
                {
                    Codigo = model.Codigo,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Activa = model.Activa,
                    FechaCreacion = DateTime.Now
                };

                _context.Carreras.Add(carrera);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Carreras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();

            var model = new CarreraViewModel
            {
                CarreraId = carrera.CarreraId,
                Codigo = carrera.Codigo,
                Nombre = carrera.Nombre,
                Descripcion = carrera.Descripcion,
                Activa = carrera.Activa
            };

            return View(model);
        }

        // POST: Carreras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarreraViewModel model)
        {
            if (id != model.CarreraId) return NotFound();

            if (ModelState.IsValid)
            {
                var carrera = await _context.Carreras.FindAsync(id);
                if (carrera == null) return NotFound();

                var existeCodigo = await _context.Carreras
                    .AnyAsync(c => c.Codigo == model.Codigo && c.CarreraId != model.CarreraId);

                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Ya existe otra carrera con ese código.");
                    return View(model);
                }

                carrera.Codigo = model.Codigo;
                carrera.Nombre = model.Nombre;
                carrera.Descripcion = model.Descripcion;
                carrera.Activa = model.Activa;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Carreras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null) return NotFound();

            return View(carrera);
        }

        // POST: Carreras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();

            _context.Carreras.Remove(carrera);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Carreras/Malla/5
        public async Task<IActionResult> Malla(int? id)
        {
            if (id == null) return NotFound();

            var carrera = await _context.Carreras
                .Include(c => c.CarreraCursos)
                    .ThenInclude(cc => cc.Curso)
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null) return NotFound();

            ViewBag.CarreraNombre = carrera.Nombre;
            ViewBag.CarreraId = carrera.CarreraId;

            var cursosDisponibles = await _context.Cursos
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CursoId = new SelectList(cursosDisponibles, "CursoId", "Nombre");

            var model = new CarreraCursoViewModel
            {
                CarreraId = carrera.CarreraId,
                Nivel = 1,
                CuatrimestreSugerido = 1
            };
            ViewBag.CursosMalla = carrera.CarreraCursos.ToList();

            return View(model);
        }

        // POST: Carreras/Malla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Malla(CarreraCursoViewModel model)
        {
            var carrera = await _context.Carreras
                .Include(c => c.CarreraCursos)
                    .ThenInclude(cc => cc.Curso)
                .FirstOrDefaultAsync(c => c.CarreraId == model.CarreraId);

            if (carrera == null) return NotFound();

            var yaExiste = await _context.CarreraCursos
                .AnyAsync(cc => cc.CarreraId == model.CarreraId && cc.CursoId == model.CursoId);

            if (yaExiste)
            {
                ModelState.AddModelError(string.Empty, "Ese curso ya está asociado a la carrera.");
            }

            if (ModelState.IsValid)
            {
                var carreraCurso = new CarreraCurso
                {
                    CarreraId = model.CarreraId,
                    CursoId = model.CursoId,
                    Nivel = model.Nivel,
                    CuatrimestreSugerido = model.CuatrimestreSugerido
                };

                _context.CarreraCursos.Add(carreraCurso);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Malla), new { id = model.CarreraId });
            }

            ViewBag.CarreraNombre = carrera.Nombre;
            ViewBag.CarreraId = carrera.CarreraId;

            var cursosDisponibles = await _context.Cursos
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CursoId = new SelectList(cursosDisponibles, "CursoId", "Nombre", model.CursoId);
            ViewBag.CursosMalla = carrera.CarreraCursos.ToList();

            return View(model);
        }

        // POST: Carreras/EliminarCursoMalla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCursoMalla(int carreraId, int cursoId)
        {
            var relacion = await _context.CarreraCursos
                .FirstOrDefaultAsync(cc => cc.CarreraId == carreraId && cc.CursoId == cursoId);

            if (relacion == null) return NotFound();

            _context.CarreraCursos.Remove(relacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Malla), new { id = carreraId });
        }
    }


}
