namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class MatriculaViewModel
    {
        public int MatriculaId { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string PeriodoNombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaConfirmada { get; set; }

        public List<OfertaCursoViewModel> CursosMatriculados { get; set; } = new();
    }
}