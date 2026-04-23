namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    public class MatriculaDetalle
    {
        public int MatriculaDetalleId { get; set; }

        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        public DateTime FechaAgregado { get; set; } = DateTime.Now;
    }
}