using System;
using System.Collections.Generic;

namespace MyApiUCI.Models;

public partial class Formulario
{
    public int Id { get; set; }

    public int? EstudianteId { get; set; }

    public int? DepartamentoId { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public bool? Firmado { get; set; }

    public byte[]? FirmaEncargado { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public virtual Departamento? Departamento { get; set; }

    public virtual Estudiante? Estudiante { get; set; }
}
