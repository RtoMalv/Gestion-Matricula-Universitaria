using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class DocenteViewModel
    {
        public int? DocentePerfilId { get; set; }

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

        [StringLength(100)]
        [Display(Name = "Especialidad")]
        public string? Especialidad { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }
    }
}