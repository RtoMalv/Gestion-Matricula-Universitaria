using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class PeriodoAcademicoViewModel
    {
        public int PeriodoAcademicoId { get; set; }

        [Required]
        [StringLength(80)]
        [Display(Name = "Nombre del período")]
        public string NombrePeriodo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha final")]
        public DateTime FechaFinal { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}