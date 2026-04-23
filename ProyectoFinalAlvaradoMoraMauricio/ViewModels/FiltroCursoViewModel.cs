using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class FiltroCursoViewModel
    {
        public int? PeriodoAcademicoId { get; set; }
        public string? Search { get; set; }
        public string? Modalidad { get; set; }
        public int? DocentePerfilId { get; set; }

        public List<OfertaCursoViewModel> Resultados { get; set; } = new();

        public SelectList? Periodos { get; set; }
        public SelectList? Docentes { get; set; }
        public SelectList? Modalidades { get; set; }
    }
}