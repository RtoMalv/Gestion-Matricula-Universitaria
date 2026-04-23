using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DocentesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocentesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.DocentesPerfil
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.User.Nombre.Contains(search) ||
                    d.User.Apellidos.Contains(search) ||
                    d.User.Email!.Contains(search) ||
                    d.Especialidad!.Contains(search));
            }

            var docentes = await query
                .OrderBy(d => d.User.Nombre)
                .ThenBy(d => d.User.Apellidos)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(docentes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var docente = await _context.DocentesPerfil
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocentePerfilId == id);

            if (docente == null) return NotFound();

            return View(docente);
        }

        public IActionResult Create()
        {
            return View(new DocenteViewModel { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocenteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeCorreo = await _userManager.FindByEmailAsync(model.Email);
                if (existeCorreo != null)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese correo.");
                }

                var existeCedula = await _context.Users.AnyAsync(u => u.Cedula == model.Cedula);
                if (existeCedula)
                {
                    ModelState.AddModelError("Cedula", "Ya existe un usuario con esa cédula.");
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Debe ingresar una contraseña.");
                }

                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Cedula = model.Cedula,
                        Nombre = model.Nombre,
                        Apellidos = model.Apellidos,
                        Direccion = model.Direccion,
                        PhoneNumber = model.Telefono,
                        Activo = model.Activo,
                        FechaIngreso = DateTime.Now,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password!);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Docente");

                        var docentePerfil = new DocentePerfil
                        {
                            UserId = user.Id,
                            Especialidad = model.Especialidad,
                            Activo = model.Activo
                        };

                        _context.DocentesPerfil.Add(docentePerfil);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var docente = await _context.DocentesPerfil
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocentePerfilId == id);

            if (docente == null) return NotFound();

            var model = new DocenteViewModel
            {
                DocentePerfilId = docente.DocentePerfilId,
                Cedula = docente.User.Cedula,
                Nombre = docente.User.Nombre,
                Apellidos = docente.User.Apellidos,
                Email = docente.User.Email ?? string.Empty,
                Telefono = docente.User.PhoneNumber,
                Direccion = docente.User.Direccion,
                Especialidad = docente.Especialidad,
                Activo = docente.Activo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocenteViewModel model)
        {
            if (model.DocentePerfilId != id) return NotFound();

            var docente = await _context.DocentesPerfil
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocentePerfilId == id);

            if (docente == null) return NotFound();

            if (ModelState.IsValid)
            {
                var existeCedula = await _context.Users
                    .AnyAsync(u => u.Cedula == model.Cedula && u.Id != docente.UserId);

                if (existeCedula)
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro usuario con esa cédula.");
                }

                var existeCorreo = await _context.Users
                    .AnyAsync(u => u.Email == model.Email && u.Id != docente.UserId);

                if (existeCorreo)
                {
                    ModelState.AddModelError("Email", "Ya existe otro usuario con ese correo.");
                }

                if (ModelState.IsValid)
                {
                    docente.User.Cedula = model.Cedula;
                    docente.User.Nombre = model.Nombre;
                    docente.User.Apellidos = model.Apellidos;
                    docente.User.Email = model.Email;
                    docente.User.UserName = model.Email;
                    docente.User.PhoneNumber = model.Telefono;
                    docente.User.Direccion = model.Direccion;
                    docente.User.Activo = model.Activo;

                    docente.Especialidad = model.Especialidad;
                    docente.Activo = model.Activo;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var docente = await _context.DocentesPerfil
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocentePerfilId == id);

            if (docente == null) return NotFound();

            return View(docente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docente = await _context.DocentesPerfil
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocentePerfilId == id);

            if (docente == null) return NotFound();

            _context.DocentesPerfil.Remove(docente);
            _context.Users.Remove(docente.User);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
