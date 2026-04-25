using System;
using System.Collections.Generic;

namespace Lab06_AlexandroCano.Models;

public partial class Asistencia
{
    public int IdAsistencia { get; set; }

    public int IdEstudiante { get; set; }

    public int IdCurso { get; set; }

    public DateOnly Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Curso IdCursoNavigation { get; set; } = null!;

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;
}
