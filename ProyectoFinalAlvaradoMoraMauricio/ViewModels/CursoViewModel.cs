using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class CursoViewModel
    {
        public int CursoId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        [Display(Name = "Nombre del curso")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Range(1, 10)]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}