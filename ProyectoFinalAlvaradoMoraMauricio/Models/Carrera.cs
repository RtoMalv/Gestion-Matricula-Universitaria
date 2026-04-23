using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Carrera
    {
        public int CarreraId { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;

        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
        public ICollection<CarreraCurso> CarreraCursos { get; set; } = new List<CarreraCurso>();
    }
}