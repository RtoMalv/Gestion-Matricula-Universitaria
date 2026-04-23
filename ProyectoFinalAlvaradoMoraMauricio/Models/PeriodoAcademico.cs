using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class PeriodoAcademico
    {
        public int PeriodoAcademicoId { get; set; }

        [Required]
        [StringLength(80)]
        public string NombrePeriodo { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}