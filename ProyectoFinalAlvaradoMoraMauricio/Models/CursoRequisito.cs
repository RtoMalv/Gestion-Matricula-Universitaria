using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class CursoRequisito
    {
        public int CursoRequisitoId { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        public int CursoRequisitoIdFk { get; set; }
        public Curso CursoRequisitoNavigation { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string TipoRequisito { get; set; } = "Requisito";
    }
}