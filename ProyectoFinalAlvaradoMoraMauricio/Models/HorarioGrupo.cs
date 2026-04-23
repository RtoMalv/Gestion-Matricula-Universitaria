using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class HorarioGrupo
    {
        public int HorarioGrupoId { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string DiaSemana { get; set; } = string.Empty;

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFinal { get; set; }
    }
}