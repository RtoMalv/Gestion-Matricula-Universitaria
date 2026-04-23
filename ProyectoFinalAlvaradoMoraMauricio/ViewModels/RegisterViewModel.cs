using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class RegisterViewModel
    {
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
        public string Email { get; set; } = string.Empty;

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

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La confirmación no coincide con la contraseña.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}