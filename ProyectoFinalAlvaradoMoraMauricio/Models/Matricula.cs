using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Matricula
    {
        public int MatriculaId { get; set; }

        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; } = null!;

        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico PeriodoAcademico { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaConfirmada { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Borrador";

        public ICollection<MatriculaDetalle> Detalles { get; set; } = new List<MatriculaDetalle>();

        // Compatibilidad temporal con código viejo
        [NotMapped]
        public int CursoId { get; set; }

        [NotMapped]
        public Curso? Curso { get; set; }

        [NotMapped]
        public DateTime FechaMatricula
        {
            get => FechaCreacion;
            set => FechaCreacion = value;
        }
    }
}