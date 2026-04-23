using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["CarreraId"] = new SelectList(
                _context.Carreras.Where(c => c.Activa).OrderBy(c => c.Nombre),
                "CarreraId",
                "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeCorreo = await _userManager.FindByEmailAsync(model.Email);
                if (existeCorreo != null)
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una cuenta registrada con ese correo.");
                }

                var existeCedula = await _context.Users.AnyAsync(u => u.Cedula == model.Cedula);
                if (existeCedula)
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una cuenta registrada con esa cédula.");
                }

                var existeCarnet = await _context.Estudiantes.AnyAsync(e => e.Carnet == model.Carnet);
                if (existeCarnet)
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un estudiante registrado con ese carnet.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    var usuario = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Cedula = model.Cedula,
                        Nombre = model.Nombre,
                        Apellidos = model.Apellidos,
                        Direccion = model.Direccion,
                        PhoneNumber = model.Telefono,
                        Activo = true,
                        FechaIngreso = DateTime.Now,
                        EmailConfirmed = true
                    };

                    var resultado = await _userManager.CreateAsync(usuario, model.Password);

                    if (resultado.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(usuario, "Estudiante");

                        var estudiante = new Estudiante
                        {
                            UserId = usuario.Id,
                            CarreraId = model.CarreraId,
                            Carnet = model.Carnet,
                            Activo = true
                        };

                        _context.Estudiantes.Add(estudiante);
                        await _context.SaveChangesAsync();

                        await _signInManager.SignInAsync(usuario, isPersistent: false);

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewData["CarreraId"] = new SelectList(
                _context.Carreras.Where(c => c.Activa).OrderBy(c => c.Nombre),
                "CarreraId",
                "Nombre",
                model.CarreraId);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(model.Email);

                if (usuario == null || !usuario.Activo)
                {
                    ModelState.AddModelError(string.Empty, "No existe una cuenta activa con ese correo.");
                    return View(model);
                }

                var resultado = await _signInManager.PasswordSignInAsync(
                    usuario.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Las credenciales ingresadas no son válidas.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}