using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class CarreraCursoViewModel
    {
        [Required]
        public int CarreraId { get; set; }

        [Required]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [Range(1, 20)]
        [Display(Name = "Nivel")]
        public int Nivel { get; set; }

        [Range(1, 12)]
        [Display(Name = "Cuatrimestre sugerido")]
        public int CuatrimestreSugerido { get; set; }
    }
}