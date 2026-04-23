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
    public class EstudiantesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstudiantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estudiantes
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.User.Nombre.Contains(search) ||
                    e.User.Apellidos.Contains(search) ||
                    (e.User.Email != null && e.User.Email.Contains(search)) ||
                    e.Carnet.Contains(search) ||
                    e.Carrera.Nombre.Contains(search));
            }

            var estudiantes = await query
                .OrderBy(e => e.User.Nombre)
                .ThenBy(e => e.User.Apellidos)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(estudiantes);
        }

        // GET: Estudiantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null) return NotFound();

            return View(estudiante);
        }

        // GET: Estudiantes/Create
        public IActionResult Create()
        {
            TempData["Info"] = "La creación de estudiantes se realiza desde el registro de cuenta.";
            return RedirectToAction("Register", "Account");
        }

        // POST: Estudiantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EstudianteAdminViewModel model)
        {
            TempData["Info"] = "La creación de estudiantes se realiza desde el registro de cuenta.";
            return RedirectToAction("Register", "Account");
        }

        // GET: Estudiantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes
                .Include(e => e.User)
                .Include(e => e.Carrera)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null) return NotFound();

            var model = new EstudianteAdminViewModel
            {
                EstudianteId = estudiante.EstudianteId,
                UserId = estudiante.UserId,
                Cedula = estudiante.User.Cedula,
                Nombre = estudiante.User.Nombre,
                Apellidos = estudiante.User.Apellidos,
                Correo = estudiante.User.Email ?? string.Empty,
                Telefono = estudiante.User.PhoneNumber,
                Direccion = estudiante.User.Direccion,
                CarreraId = estudiante.CarreraId,
                Carnet = estudiante.Carnet,
                Activo = estudiante.Activo
            };

            ViewData["CarreraId"] = new SelectList(
                _context.Carreras.OrderBy(c => c.Nombre),
                "CarreraId",
                "Nombre",
                model.CarreraId);

            return View(model);
        }

        // POST: Estudiantes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstudianteAdminViewModel model)
        {
            if (id != model.EstudianteId) return NotFound();

            var estudiante = await _context.Estudiantes
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null) return NotFound();

            if (ModelState.IsValid)
            {
                var existeCedula = await _context.Users
                    .AnyAsync(u => u.Cedula == model.Cedula && u.Id != estudiante.UserId);

                if (existeCedula)
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro usuario con esa cédula.");
                }

                var existeCorreo = await _context.Users
                    .AnyAsync(u => u.Email == model.Correo && u.Id != estudiante.UserId);

                if (existeCorreo)
                {
                    ModelState.AddModelError("Correo", "Ya existe otro usuario con ese correo.");
                }

                var existeCarnet = await _context.Estudiantes
                    .AnyAsync(e => e.Carnet == model.Carnet && e.EstudianteId != model.EstudianteId);

                if (existeCarnet)
                {
                    ModelState.AddModelError("Carnet", "Ya existe otro estudiante con ese carnet.");
                }

                if (ModelState.IsValid)
                {
                    estudiante.User.Cedula = model.Cedula;
                    estudiante.User.Nombre = model.Nombre;
                    estudiante.User.Apellidos = model.Apellidos;
                    estudiante.User.Email = model.Correo;
                    estudiante.User.UserName = model.Correo;
                    estudiante.User.PhoneNumber = model.Telefono;
                    estudiante.User.Direccion = model.Direccion;
                    estudiante.User.Activo = model.Activo;

                    estudiante.CarreraId = model.CarreraId;
                    estudiante.Carnet = model.Carnet;
                    estudiante.Activo = model.Activo;

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["CarreraId"] = new SelectList(
                _context.Carreras.OrderBy(c => c.Nombre),
                "CarreraId",
                "Nombre",
                model.CarreraId);

            return View(model);
        }

        // GET: Estudiantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null) return NotFound();

            return View(estudiante);
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null) return NotFound();

            // Desactivación lógica
            estudiante.Activo = false;
            estudiante.User.Activo = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EstudianteExists(int id)
        {
            return _context.Estudiantes.Any(e => e.EstudianteId == id);
        }
    }
}