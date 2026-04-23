using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Curso
    {
        public int CursoId { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Descripcion { get; set; }

        [Range(1, 10)]
        public int Creditos { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<CarreraCurso> CarreraCursos { get; set; } = new List<CarreraCurso>();
        public ICollection<CursoRequisito> RequisitosDelCurso { get; set; } = new List<CursoRequisito>();
        public ICollection<CursoRequisito> EsRequisitoDe { get; set; } = new List<CursoRequisito>();
        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

        // Compatibilidad temporal con scaffolding viejo
        [NotMapped]
        public int? CarreraId { get; set; }

        [NotMapped]
        public Carrera? Carrera { get; set; }

        [NotMapped]
        public string NombreMostrar => $"{Codigo} - {Nombre}";
    }
}