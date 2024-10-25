using System;
using System.Collections.Generic;

namespace MyApiUCI.Models;

public partial class Carrera
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int? FacultadId { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public virtual Facultad? Facultad { get; set; }
}
