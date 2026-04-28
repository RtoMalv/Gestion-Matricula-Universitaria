namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class ComprobanteMatriculaViewModel
    {
        public int MatriculaId { get; set; }

        public string NombreEstudiante { get; set; } = string.Empty;
        public string Carnet { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;

        public string PeriodoNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaConfirmada { get; set; }

        public int TotalCursos { get; set; }
        public int TotalCreditos { get; set; }

        public List<OfertaCursoViewModel> Cursos { get; set; } = new();
    }
}