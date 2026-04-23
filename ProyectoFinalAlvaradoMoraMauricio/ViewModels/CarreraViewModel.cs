using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class CarreraViewModel
    {
        public int CarreraId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre de la carrera")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;
    }
}