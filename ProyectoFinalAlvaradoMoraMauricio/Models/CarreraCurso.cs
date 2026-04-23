namespace ProyectoFinalAlvaradoMoraMauricio.Models
{
    // Relación entre carrera y curso para construir la malla
    public class CarreraCurso
    {
        public int CarreraId { get; set; }
        public Carrera Carrera { get; set; } = null!;

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        public int Nivel { get; set; }  // Ejemplo: 1, 2, 3...
        public int CuatrimestreSugerido { get; set; }
    }
}
