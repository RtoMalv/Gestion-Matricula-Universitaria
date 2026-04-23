using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class EstudianteAdminViewModel
    {
        public int EstudianteId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Required]
        [Display(Name = "Carrera")]
        public int CarreraId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Carnet")]
        public string Carnet { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
