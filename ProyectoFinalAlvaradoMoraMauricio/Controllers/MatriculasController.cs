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

            filtro.Periodos = new SelectList(
                periodos,
                "PeriodoAcademicoId",
                "NombrePeriodo",
                filtro.PeriodoAcademicoId);

            filtro.Docentes = new SelectList(
                docentes,
                "DocentePerfilId",
                "NombreMostrar",
                filtro.DocentePerfilId);

            filtro.Modalidades = new SelectList(
                new List<string> { "Presencial", "Virtual", "Híbrida" },
                filtro.Modalidad);

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
                    HorarioTexto = horario != null
                        ? $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFinal:hh\\:mm}"
                        : null
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
            if (string.IsNullOrWhiteSpace(userEmail))
                return Challenge();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Challenge();

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null)
            {
                TempData["Error"] = "No se encontró el perfil del estudiante.";
                return RedirectToAction(nameof(Oferta));
            }

            var grupo = await _context.Grupos
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.Curso)
                .Include(g => g.Horarios)
                .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

            if (grupo == null)
            {
                TempData["Error"] = "No se encontró el grupo seleccionado.";
                return RedirectToAction(nameof(Oferta));
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Grupo)
                        .ThenInclude(g => g.Curso)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Grupo)
                        .ThenInclude(g => g.Horarios)
                .FirstOrDefaultAsync(m =>
                    m.EstudianteId == estudiante.EstudianteId &&
                    m.PeriodoAcademicoId == grupo.PeriodoAcademicoId);

            if (matricula != null && matricula.Estado == "Confirmada")
            {
                TempData["Error"] = "La matrícula de este período ya fue confirmada y no puede modificarse.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

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

                matricula = await _context.Matriculas
                    .Include(m => m.Detalles)
                        .ThenInclude(d => d.Grupo)
                            .ThenInclude(g => g.Curso)
                    .Include(m => m.Detalles)
                        .ThenInclude(d => d.Grupo)
                            .ThenInclude(g => g.Horarios)
                    .FirstAsync(m => m.MatriculaId == matricula.MatriculaId);
            }

            var yaExisteGrupo = await _context.MatriculaDetalles.AnyAsync(md =>
                md.MatriculaId == matricula.MatriculaId &&
                md.GrupoId == grupoId);

            if (yaExisteGrupo)
            {
                TempData["Info"] = "El grupo seleccionado ya forma parte de su matrícula.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            var yaTieneMismoCurso = await _context.MatriculaDetalles
                .Include(md => md.Grupo)
                .AnyAsync(md =>
                    md.MatriculaId == matricula.MatriculaId &&
                    md.Grupo.CursoId == grupo.CursoId);

            if (yaTieneMismoCurso)
            {
                TempData["Error"] = "Ya tiene matriculado otro grupo de ese mismo curso en este período.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            var horariosNuevoGrupo = grupo.Horarios.ToList();

            foreach (var detalleExistente in matricula.Detalles)
            {
                var grupoExistente = detalleExistente.Grupo;
                var horariosExistentes = grupoExistente.Horarios.ToList();

                foreach (var nuevo in horariosNuevoGrupo)
                {
                    foreach (var existente in horariosExistentes)
                    {
                        var mismoDia = nuevo.DiaSemana == existente.DiaSemana;
                        var hayTraslape = nuevo.HoraInicio < existente.HoraFinal &&
                                          nuevo.HoraFinal > existente.HoraInicio;

                        if (mismoDia && hayTraslape)
                        {
                            TempData["Error"] =
                                $"Choque de horario detectado con el curso {grupoExistente.Curso.Codigo} - {grupoExistente.Curso.Nombre}, grupo {grupoExistente.NumeroGrupo}.";
                            return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
                        }
                    }
                }
            }

            _context.MatriculaDetalles.Add(new MatriculaDetalle
            {
                MatriculaId = matricula.MatriculaId,
                GrupoId = grupoId,
                FechaAgregado = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "El grupo fue agregado correctamente a la matrícula.";
            return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> MiMatricula(int? matriculaId, int? periodoId)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
                return Challenge();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Challenge();

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null)
                return NotFound("No se encontró el perfil del estudiante.");

            IQueryable<Matricula> query = _context.Matriculas
                .Include(m => m.PeriodoAcademico)
                .Where(m => m.EstudianteId == estudiante.EstudianteId);

            Matricula? matricula = null;

            if (matriculaId.HasValue)
            {
                matricula = await query
                    .FirstOrDefaultAsync(m => m.MatriculaId == matriculaId.Value);
            }
            else if (periodoId.HasValue)
            {
                matricula = await query
                    .FirstOrDefaultAsync(m => m.PeriodoAcademicoId == periodoId.Value);
            }
            else
            {
                matricula = await query
                    .Where(m => m.Estado == "Borrador")
                    .OrderByDescending(m => m.FechaCreacion)
                    .FirstOrDefaultAsync();

                if (matricula == null)
                {
                    matricula = await query
                        .OrderByDescending(m => m.FechaCreacion)
                        .FirstOrDefaultAsync();
                }
            }

            if (matricula == null)
            {
                return View(new MatriculaViewModel());
            }

            var detalles = await _context.MatriculaDetalles
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.Curso)
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.DocentePerfil)
                        .ThenInclude(d => d!.User)
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.Horarios)
                .Where(md => md.MatriculaId == matricula.MatriculaId)
                .OrderBy(md => md.Grupo.Curso.Nombre)
                .ThenBy(md => md.Grupo.NumeroGrupo)
                .ToListAsync();

            var model = new MatriculaViewModel
            {
                MatriculaId = matricula.MatriculaId,
                Estado = matricula.Estado,
                PeriodoNombre = matricula.PeriodoAcademico.NombrePeriodo,
                FechaCreacion = matricula.FechaCreacion,
                FechaConfirmada = matricula.FechaConfirmada,
                CursosMatriculados = detalles.Select(d =>
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
                        HorarioTexto = horario != null
                            ? $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFinal:hh\\:mm}"
                            : null
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
            var matricula = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.MatriculaId == matriculaId);

            if (matricula == null)
                return NotFound();

            if (matricula.Estado == "Confirmada")
            {
                TempData["Error"] = "No puede quitar cursos de una matrícula ya confirmada.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            var detalle = await _context.MatriculaDetalles
                .FirstOrDefaultAsync(md => md.MatriculaId == matriculaId && md.GrupoId == grupoId);

            if (detalle == null)
            {
                TempData["Error"] = "No se encontró el grupo dentro de la matrícula.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            _context.MatriculaDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            TempData["Success"] = "El grupo fue retirado de la matrícula.";
            return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
        }

        [Authorize(Roles = "Estudiante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int matriculaId)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Detalles)
                .FirstOrDefaultAsync(m => m.MatriculaId == matriculaId);

            if (matricula == null)
                return NotFound();

            if (matricula.Estado == "Confirmada")
            {
                TempData["Info"] = "La matrícula ya se encontraba confirmada.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            if (!matricula.Detalles.Any())
            {
                TempData["Error"] = "No puede confirmar una matrícula vacía.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            matricula.Estado = "Confirmada";
            matricula.FechaConfirmada = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "La matrícula fue confirmada correctamente.";
            return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> Comprobante(int id)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
                return Challenge();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Challenge();

            var estudiante = await _context.Estudiantes
                .Include(e => e.User)
                .Include(e => e.Carrera)
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null)
                return NotFound("No se encontró el perfil del estudiante.");

            var matricula = await _context.Matriculas
                .Include(m => m.PeriodoAcademico)
                .FirstOrDefaultAsync(m =>
                    m.MatriculaId == id &&
                    m.EstudianteId == estudiante.EstudianteId);

            if (matricula == null)
                return NotFound();

            if (matricula.Estado != "Confirmada")
            {
                TempData["Error"] = "Solo puede generar el comprobante de una matrícula confirmada.";
                return RedirectToAction(nameof(MiMatricula), new { matriculaId = matricula.MatriculaId });
            }

            var detalles = await _context.MatriculaDetalles
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.Curso)
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.DocentePerfil)
                        .ThenInclude(d => d!.User)
                .Include(md => md.Grupo)
                    .ThenInclude(g => g.Horarios)
                .Where(md => md.MatriculaId == matricula.MatriculaId)
                .OrderBy(md => md.Grupo.Curso.Nombre)
                .ThenBy(md => md.Grupo.NumeroGrupo)
                .ToListAsync();

            var cursos = detalles.Select(d =>
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
                    HorarioTexto = horario != null
                        ? $"{horario.HoraInicio:hh\\:mm} - {horario.HoraFinal:hh\\:mm}"
                        : null
                };
            }).ToList();

            var model = new ComprobanteMatriculaViewModel
            {
                MatriculaId = matricula.MatriculaId,
                NombreEstudiante = estudiante.User.NombreCompleto,
                Carnet = estudiante.Carnet,
                Correo = estudiante.User.Email ?? string.Empty,
                Carrera = estudiante.Carrera.Nombre,
                PeriodoNombre = matricula.PeriodoAcademico.NombrePeriodo,
                Estado = matricula.Estado,
                FechaCreacion = matricula.FechaCreacion,
                FechaConfirmada = matricula.FechaConfirmada,
                TotalCursos = cursos.Count,
                TotalCreditos = detalles.Sum(d => d.Grupo.Curso.Creditos),
                Cursos = cursos
            };

            return View(model);
        }
    }
}