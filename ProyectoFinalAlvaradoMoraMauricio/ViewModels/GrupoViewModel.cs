using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class GrupoViewModel
    {
        public int GrupoId { get; set; }

        [Required]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [Required]
        [Display(Name = "Período académico")]
        public int PeriodoAcademicoId { get; set; }

        [Display(Name = "Docente")]
        public int? DocentePerfilId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Número de grupo")]
        public string NumeroGrupo { get; set; } = string.Empty;

        [Range(1, 100)]
        [Display(Name = "Cupo máximo")]
        public int CupoMaximo { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Modalidad")]
        public string Modalidad { get; set; } = "Presencial";

        [Required]
        [StringLength(30)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        [Display(Name = "Aula")]
        public string? Aula { get; set; }

        [Required]
        [Display(Name = "Día")]
        public string DiaSemana { get; set; } = "Lunes";

        [Required]
        [Display(Name = "Hora de inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora final")]
        public TimeSpan HoraFinal { get; set; }
    }
}