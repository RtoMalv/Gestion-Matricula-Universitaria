using Microsoft.AspNetCore.Identity;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
    }
}