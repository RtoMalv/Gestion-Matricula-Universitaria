using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Estudiante
    {
        public int EstudianteId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int CarreraId { get; set; }
        public Carrera Carrera { get; set; } = null!;

        public string Carnet { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        // Compatibilidad temporal con vistas/controladores anteriores
        [NotMapped]
        public string Nombre
        {
            get => User != null ? User.NombreCompleto : string.Empty;
            set { }
        }

        [NotMapped]
        public string Correo
        {
            get => User != null ? User.Email ?? string.Empty : string.Empty;
            set { }
        }

        [NotMapped]
        public string Telefono
        {
            get => User != null ? User.PhoneNumber ?? string.Empty : string.Empty;
            set { }
        }

        [NotMapped]
        public string Direccion
        {
            get => User != null ? User.Direccion ?? string.Empty : string.Empty;
            set { }
        }
    }
}