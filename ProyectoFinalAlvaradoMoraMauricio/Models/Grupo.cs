using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Grupo
    {
        public int GrupoId { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico PeriodoAcademico { get; set; } = null!;

        public int? DocentePerfilId { get; set; }
        public DocentePerfil? DocentePerfil { get; set; }

        [Required]
        [StringLength(10)]
        public string NumeroGrupo { get; set; } = string.Empty;

        [Range(1, 100)]
        public int CupoMaximo { get; set; }

        [StringLength(30)]
        public string Modalidad { get; set; } = "Presencial";

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        [StringLength(50)]
        public string? Aula { get; set; }

        public ICollection<HorarioGrupo> Horarios { get; set; } = new List<HorarioGrupo>();
        public ICollection<MatriculaDetalle> MatriculaDetalles { get; set; } = new List<MatriculaDetalle>();
    }
}
