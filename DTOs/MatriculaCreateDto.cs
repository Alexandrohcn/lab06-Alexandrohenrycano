using System.ComponentModel.DataAnnotations;

namespace Lab06_AlexandroCano.DTOs
{
    public class MatriculaCreateDto
    {
        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdCurso { get; set; }

        [Required]
        public int IdProfesor { get; set; }

        [Required]
        [MaxLength(20)]
        public string Semestre { get; set; } = null!;
    }
}