using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class DocentePerfil
    {
        public int DocentePerfilId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [StringLength(100)]
        public string? Especialidad { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

        [NotMapped]
        public string NombreMostrar => User != null ? $"{User.NombreCompleto} ({User.Email})" : string.Empty;
    }
}