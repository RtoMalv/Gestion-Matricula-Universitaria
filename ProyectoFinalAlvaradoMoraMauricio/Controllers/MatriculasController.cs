using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Data;
using ProyectoFinalAlvaradoMoraMauricio.Models;
using ProyectoFinalAlvaradoMoraMauricio.ViewModels;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatriculasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> Oferta(FiltroCursoViewModel filtro)
        {
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();

            var docentes = await _context.DocentesPerfil
                .Include(d => d.User)
                .Where(d => d.Activo)
                .OrderBy(d => d.User.Nombre)
                .ToListAsync();

            filtro.Periodos = new SelectList(periodos, "PeriodoAcademicoId", "NombrePeriodo", filtro.PeriodoAcademicoId);
            filtro.Docentes = new SelectList(docentes, "DocentePerfilId", "NombreMostrar", filtro.DocentePerfilId);
            filtro.Modalidades = new SelectList(new List<string> { "Presencial", "Virtual", "Híbrida" }, filtro.Modalidad);

            var query = _context.Grupos
                .Include(g => g.Curso)
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.DocentePerfil)
                    .ThenInclude(d => d!.User)
                .Include(g => g.Horarios)
                .Where(g => g.Estado == "Activo")
                .AsQueryable();

            if (filtro.PeriodoAcademicoId.HasValue)
            {
                query = query.Where(g => g.PeriodoAcademicoId == filtro.PeriodoAcademicoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                query = query.Where(g =>
                    g.Curso.Nombre.Contains(filtro.Search) ||
                    g.Curso.Codigo.Contains(filtro.Search));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Modalidad))
            {
                query = query.Where(g => g.Modalidad == filtro.Modalidad);
            }

            if (filtro.DocentePerfilId.HasValue)
            {
                query = query.Where(g => g.DocentePerfilId == filtro.DocentePerfilId.Value);
            }

            var grupos = await query
                .OrderBy(g => g.Curso.Nombre)
                .ThenBy(g => g.NumeroGrupo)
                .ToListAsync();

            filtro.Resultados = grupos.Select(g =>
            {
                var horario = g.Horarios.FirstOrDefault();

                return new OfertaCursoViewModel
                {
                    GrupoId = g.GrupoId,
                    CursoCodigo = g.Curso.Codigo,
                    CursoNombre = g.Curso.Nombre,
                    NumeroGrupo = g.NumeroGrupo,
                    PeriodoNombre = g.PeriodoAcademico.NombrePeriodo,
                    DocenteNombre = g.DocentePerfil?.User?.NombreCompleto,
                    Modalidad = g.Modalidad,
                    Aula = g.Aula,
                    CupoMaximo = g.CupoMaximo,
                    DiaSemana = horario?.DiaSemana,
                    HorarioTexto = horario != null ? $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFinal:hh\\:mm}" : null
                };
            }).ToList();

            return View(filtro);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarGrupo(int grupoId)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail)) return Challenge();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Challenge();

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null) return NotFound("No se encontró el perfil del estudiante.");

            var grupo = await _context.Grupos
                .Include(g => g.PeriodoAcademico)
                .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

            if (grupo == null) return NotFound();

            var matricula = await _context.Matriculas
                .Include(m => m.Detalles)
                .FirstOrDefaultAsync(m =>
                    m.EstudianteId == estudiante.EstudianteId &&
                    m.PeriodoAcademicoId == grupo.PeriodoAcademicoId &&
                    m.Estado == "Borrador");

            if (matricula == null)
            {
                matricula = new Matricula
                {
                    EstudianteId = estudiante.EstudianteId,
                    PeriodoAcademicoId = grupo.PeriodoAcademicoId,
                    FechaCreacion = DateTime.Now,
                    Estado = "Borrador"
                };

                _context.Matriculas.Add(matricula);
                await _context.SaveChangesAsync();
            }

            var yaExiste = await _context.MatriculaDetalles.AnyAsync(md =>
                md.MatriculaId == matricula.MatriculaId &&
                md.GrupoId == grupoId);

            if (!yaExiste)
            {
                _context.MatriculaDetalles.Add(new MatriculaDetalle
                {
                    MatriculaId = matricula.MatriculaId,
                    GrupoId = grupoId,
                    FechaAgregado = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MiMatricula), new { periodoId = grupo.PeriodoAcademicoId });
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> MiMatricula(int? periodoId)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail)) return Challenge();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Challenge();

            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (estudiante == null) return NotFound("No se encontró el perfil del estudiante.");

            var query = _context.Matriculas
                .Include(m => m.PeriodoAcademico)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Grupo)
                        .ThenInclude(g => g.Curso)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Grupo)
                        .ThenInclude(g => g.DocentePerfil)
                            .ThenInclude(d => d!.User)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Grupo)
                        .ThenInclude(g => g.Horarios)
                .Where(m => m.EstudianteId == estudiante.EstudianteId)
                .AsQueryable();

            if (periodoId.HasValue)
            {
                query = query.Where(m => m.PeriodoAcademicoId == periodoId.Value);
            }
            else
            {
                query = query.OrderByDescending(m => m.FechaCreacion);
            }

            var matricula = await query.FirstOrDefaultAsync();

            if (matricula == null)
            {
                return View(new MatriculaViewModel());
            }

            var model = new MatriculaViewModel
            {
                MatriculaId = matricula.MatriculaId,
                Estado = matricula.Estado,
                PeriodoNombre = matricula.PeriodoAcademico.NombrePeriodo,
                FechaCreacion = matricula.FechaCreacion,
                FechaConfirmada = matricula.FechaConfirmada,
                CursosMatriculados = matricula.Detalles.Select(d =>
                {
                    var horario = d.Grupo.Horarios.FirstOrDefault();
                    return new OfertaCursoViewModel
                    {
                        GrupoId = d.GrupoId,
                        CursoCodigo = d.Grupo.Curso.Codigo,
                        CursoNombre = d.Grupo.Curso.Nombre,
                        NumeroGrupo = d.Grupo.NumeroGrupo,
                        PeriodoNombre = matricula.PeriodoAcademico.NombrePeriodo,
                        DocenteNombre = d.Grupo.DocentePerfil?.User?.NombreCompleto,
                        Modalidad = d.Grupo.Modalidad,
                        Aula = d.Grupo.Aula,
                        CupoMaximo = d.Grupo.CupoMaximo,
                        DiaSemana = horario?.DiaSemana,
                        HorarioTexto = horario != null ? $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFinal:hh\\:mm}" : null
                    };
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarGrupo(int matriculaId, int grupoId)
        {
            var detalle = await _context.MatriculaDetalles
                .FirstOrDefaultAsync(md => md.MatriculaId == matriculaId && md.GrupoId == grupoId);

            if (detalle == null) return NotFound();

            _context.MatriculaDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.MatriculaId == matriculaId);
            return RedirectToAction(nameof(MiMatricula), new { periodoId = matricula?.PeriodoAcademicoId });
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int matriculaId)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Detalles)
                .FirstOrDefaultAsync(m => m.MatriculaId == matriculaId);

            if (matricula == null) return NotFound();

            if (!matricula.Detalles.Any())
            {
                return RedirectToAction(nameof(MiMatricula), new { periodoId = matricula.PeriodoAcademicoId });
            }

            matricula.Estado = "Confirmada";
            matricula.FechaConfirmada = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MiMatricula), new { periodoId = matricula.PeriodoAcademicoId });
        }
    }
}