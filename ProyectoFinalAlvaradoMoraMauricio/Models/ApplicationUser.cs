using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [NotMapped]
        public string NombreCompleto
        {
            get => $"{Nombre} {Apellidos}".Trim();
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var partes = value.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    Nombre = partes.Length > 0 ? partes[0] : string.Empty;
                    Apellidos = partes.Length > 1 ? partes[1] : string.Empty;
                }
            }
        }
    }
}