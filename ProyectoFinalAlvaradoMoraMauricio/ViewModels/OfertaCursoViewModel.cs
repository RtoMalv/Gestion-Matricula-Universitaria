namespace ProyectoFinalAlvaradoMoraMauricio.ViewModels
{
    public class OfertaCursoViewModel
    {
        public int GrupoId { get; set; }
        public string CursoCodigo { get; set; } = string.Empty;
        public string CursoNombre { get; set; } = string.Empty;
        public string NumeroGrupo { get; set; } = string.Empty;
        public string PeriodoNombre { get; set; } = string.Empty;
        public string? DocenteNombre { get; set; }
        public string Modalidad { get; set; } = string.Empty;
        public string? Aula { get; set; }
        public int CupoMaximo { get; set; }
        public string? DiaSemana { get; set; }
        public string? HorarioTexto { get; set; }
    }
}
