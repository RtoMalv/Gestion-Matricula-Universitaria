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
    public class GruposController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GruposController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Grupos
                .Include(g => g.Curso)
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.DocentePerfil)
                    .ThenInclude(d => d!.User)
                .Include(g => g.Horarios)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(g =>
                    g.Curso.Nombre.Contains(search) ||
                    g.Curso.Codigo.Contains(search) ||
                    g.NumeroGrupo.Contains(search) ||
                    g.PeriodoAcademico.NombrePeriodo.Contains(search));
            }

            var grupos = await query
                .OrderBy(g => g.Curso.Nombre)
                .ThenBy(g => g.NumeroGrupo)
                .ToListAsync();

            ViewData["Search"] = search;
            return View(grupos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var grupo = await _context.Grupos
                .Include(g => g.Curso)
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.DocentePerfil)
                    .ThenInclude(d => d!.User)
                .Include(g => g.Horarios)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null) return NotFound();

            return View(grupo);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new GrupoViewModel
            {
                CupoMaximo = 25,
                Estado = "Activo",
                Modalidad = "Presencial",
                DiaSemana = "Lunes",
                HoraInicio = new TimeSpan(18, 0, 0),
                HoraFinal = new TimeSpan(21, 0, 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GrupoViewModel model)
        {
            if (model.HoraFinal <= model.HoraInicio)
            {
                ModelState.AddModelError("HoraFinal", "La hora final debe ser mayor que la hora de inicio.");
            }

            var existeGrupo = await _context.Grupos.AnyAsync(g =>
                g.CursoId == model.CursoId &&
                g.PeriodoAcademicoId == model.PeriodoAcademicoId &&
                g.NumeroGrupo == model.NumeroGrupo);

            if (existeGrupo)
            {
                ModelState.AddModelError("NumeroGrupo", "Ya existe un grupo con ese número para el curso y período seleccionados.");
            }

            if (ModelState.IsValid)
            {
                var grupo = new Grupo
                {
                    CursoId = model.CursoId,
                    PeriodoAcademicoId = model.PeriodoAcademicoId,
                    DocentePerfilId = model.DocentePerfilId,
                    NumeroGrupo = model.NumeroGrupo,
                    CupoMaximo = model.CupoMaximo,
                    Modalidad = model.Modalidad,
                    Estado = model.Estado,
                    Aula = model.Aula
                };

                _context.Grupos.Add(grupo);
                await _context.SaveChangesAsync();

                var horario = new HorarioGrupo
                {
                    GrupoId = grupo.GrupoId,
                    DiaSemana = model.DiaSemana,
                    HoraInicio = model.HoraInicio,
                    HoraFinal = model.HoraFinal
                };

                _context.HorariosGrupo.Add(horario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(model.CursoId, model.PeriodoAcademicoId, model.DocentePerfilId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var grupo = await _context.Grupos
                .Include(g => g.Horarios)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null) return NotFound();

            var horario = grupo.Horarios.FirstOrDefault();

            var model = new GrupoViewModel
            {
                GrupoId = grupo.GrupoId,
                CursoId = grupo.CursoId,
                PeriodoAcademicoId = grupo.PeriodoAcademicoId,
                DocentePerfilId = grupo.DocentePerfilId,
                NumeroGrupo = grupo.NumeroGrupo,
                CupoMaximo = grupo.CupoMaximo,
                Modalidad = grupo.Modalidad,
                Estado = grupo.Estado,
                Aula = grupo.Aula,
                DiaSemana = horario?.DiaSemana ?? "Lunes",
                HoraInicio = horario?.HoraInicio ?? new TimeSpan(18, 0, 0),
                HoraFinal = horario?.HoraFinal ?? new TimeSpan(21, 0, 0)
            };

            await CargarCombos(model.CursoId, model.PeriodoAcademicoId, model.DocentePerfilId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GrupoViewModel model)
        {
            if (id != model.GrupoId) return NotFound();

            var grupo = await _context.Grupos
                .Include(g => g.Horarios)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null) return NotFound();

            if (model.HoraFinal <= model.HoraInicio)
            {
                ModelState.AddModelError("HoraFinal", "La hora final debe ser mayor que la hora de inicio.");
            }

            var existeGrupo = await _context.Grupos.AnyAsync(g =>
                g.CursoId == model.CursoId &&
                g.PeriodoAcademicoId == model.PeriodoAcademicoId &&
                g.NumeroGrupo == model.NumeroGrupo &&
                g.GrupoId != model.GrupoId);

            if (existeGrupo)
            {
                ModelState.AddModelError("NumeroGrupo", "Ya existe otro grupo con ese número para el curso y período seleccionados.");
            }

            if (ModelState.IsValid)
            {
                grupo.CursoId = model.CursoId;
                grupo.PeriodoAcademicoId = model.PeriodoAcademicoId;
                grupo.DocentePerfilId = model.DocentePerfilId;
                grupo.NumeroGrupo = model.NumeroGrupo;
                grupo.CupoMaximo = model.CupoMaximo;
                grupo.Modalidad = model.Modalidad;
                grupo.Estado = model.Estado;
                grupo.Aula = model.Aula;

                var horario = grupo.Horarios.FirstOrDefault();
                if (horario == null)
                {
                    horario = new HorarioGrupo
                    {
                        GrupoId = grupo.GrupoId
                    };
                    _context.HorariosGrupo.Add(horario);
                }

                horario.DiaSemana = model.DiaSemana;
                horario.HoraInicio = model.HoraInicio;
                horario.HoraFinal = model.HoraFinal;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(model.CursoId, model.PeriodoAcademicoId, model.DocentePerfilId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var grupo = await _context.Grupos
                .Include(g => g.Curso)
                .Include(g => g.PeriodoAcademico)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null) return NotFound();

            return View(grupo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupo = await _context.Grupos
                .Include(g => g.Horarios)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null) return NotFound();

            if (grupo.Horarios.Any())
            {
                _context.HorariosGrupo.RemoveRange(grupo.Horarios);
            }

            _context.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? cursoId = null, int? periodoId = null, int? docenteId = null)
        {
            ViewBag.CursoId = new SelectList(
                await _context.Cursos.Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync(),
                "CursoId",
                "NombreMostrar",
                cursoId);

            ViewBag.PeriodoAcademicoId = new SelectList(
                await _context.PeriodosAcademicos.OrderByDescending(p => p.FechaInicio).ToListAsync(),
                "PeriodoAcademicoId",
                "NombrePeriodo",
                periodoId);

            ViewBag.DocentePerfilId = new SelectList(
                await _context.DocentesPerfil.Include(d => d.User).Where(d => d.Activo).OrderBy(d => d.User.Nombre).ToListAsync(),
                "DocentePerfilId",
                "NombreMostrar",
                docenteId);
        }
    }
}