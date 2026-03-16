using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class Docente
    {
        public int DocenteId { get; set; }

        [Required(ErrorMessage = "El nombre del docente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La especialidad no puede superar los 100 caracteres.")]
        public string? Especialidad { get; set; }
    }
}