using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class CursoRequisitoViewModel
    {
        [Required]
        public int CursoId { get; set; }

        [Required]
        [Display(Name = "Curso requisito")]
        public int CursoRequisitoIdFk { get; set; }

        [Required]
        [Display(Name = "Tipo de requisito")]
        public string TipoRequisito { get; set; } = "Requisito";
    }
}