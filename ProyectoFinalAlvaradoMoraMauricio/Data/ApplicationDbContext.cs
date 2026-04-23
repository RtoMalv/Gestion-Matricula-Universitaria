using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalAlvaradoMoraMauricio.Models;

namespace ProyectoFinalAlvaradoMoraMauricio.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<CarreraCurso> CarreraCursos { get; set; }
        public DbSet<CursoRequisito> CursoRequisitos { get; set; }
        public DbSet<PeriodoAcademico> PeriodosAcademicos { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<DocentePerfil> DocentesPerfil { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<HorarioGrupo> HorariosGrupo { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<MatriculaDetalle> MatriculaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Carrera
            builder.Entity<Carrera>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Curso
            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Relación malla: Carrera - Curso
            builder.Entity<CarreraCurso>()
                .HasKey(cc => new { cc.CarreraId, cc.CursoId });

            builder.Entity<CarreraCurso>()
                .HasOne(cc => cc.Carrera)
                .WithMany(c => c.CarreraCursos)
                .HasForeignKey(cc => cc.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarreraCurso>()
                .HasOne(cc => cc.Curso)
                .WithMany(c => c.CarreraCursos)
                .HasForeignKey(cc => cc.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Requisitos y correquisitos
            builder.Entity<CursoRequisito>()
                .HasOne(cr => cr.Curso)
                .WithMany(c => c.RequisitosDelCurso)
                .HasForeignKey(cr => cr.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CursoRequisito>()
                .HasOne(cr => cr.CursoRequisitoNavigation)
                .WithMany(c => c.EsRequisitoDe)
                .HasForeignKey(cr => cr.CursoRequisitoIdFk)
                .OnDelete(DeleteBehavior.Restrict);

            // Estudiante -> ApplicationUser
            builder.Entity<Estudiante>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estudiante>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            builder.Entity<Estudiante>()
                .HasIndex(e => e.Carnet)
                .IsUnique();

            builder.Entity<Estudiante>()
                .HasOne(e => e.Carrera)
                .WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // DocentePerfil -> ApplicationUser
            builder.Entity<DocentePerfil>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DocentePerfil>()
                .HasIndex(d => d.UserId)
                .IsUnique();

            // Grupo
            builder.Entity<Grupo>()
                .HasOne(g => g.Curso)
                .WithMany(c => c.Grupos)
                .HasForeignKey(g => g.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grupo>()
                .HasOne(g => g.PeriodoAcademico)
                .WithMany(p => p.Grupos)
                .HasForeignKey(g => g.PeriodoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grupo>()
                .HasOne(g => g.DocentePerfil)
                .WithMany(d => d.Grupos)
                .HasForeignKey(g => g.DocentePerfilId)
                .OnDelete(DeleteBehavior.SetNull);

            // Evita duplicar número de grupo para el mismo curso en el mismo período
            builder.Entity<Grupo>()
                .HasIndex(g => new { g.CursoId, g.PeriodoAcademicoId, g.NumeroGrupo })
                .IsUnique();

            // HorarioGrupo
            builder.Entity<HorarioGrupo>()
                .HasOne(h => h.Grupo)
                .WithMany(g => g.Horarios)
                .HasForeignKey(h => h.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Matrícula
            builder.Entity<Matricula>()
                .HasOne(m => m.Estudiante)
                .WithMany(e => e.Matriculas)
                .HasForeignKey(m => m.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Matricula>()
                .HasOne(m => m.PeriodoAcademico)
                .WithMany(p => p.Matriculas)
                .HasForeignKey(m => m.PeriodoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Una matrícula por estudiante por período
            builder.Entity<Matricula>()
                .HasIndex(m => new { m.EstudianteId, m.PeriodoAcademicoId })
                .IsUnique();

            // Detalle de matrícula
            builder.Entity<MatriculaDetalle>()
                .HasOne(md => md.Matricula)
                .WithMany(m => m.Detalles)
                .HasForeignKey(md => md.MatriculaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MatriculaDetalle>()
                .HasOne(md => md.Grupo)
                .WithMany(g => g.MatriculaDetalles)
                .HasForeignKey(md => md.GrupoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evita repetir el mismo grupo dentro de la misma matrícula
            builder.Entity<MatriculaDetalle>()
                .HasIndex(md => new { md.MatriculaId, md.GrupoId })
                .IsUnique();
        }
    }
}