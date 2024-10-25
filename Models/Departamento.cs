using System;
using System.Collections.Generic;

namespace MyApiUCI.Models;

public partial class Departamento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int? FacultadId { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public virtual ICollection<Encargado> Encargados { get; set; } = new List<Encargado>();

    public virtual Facultad? Facultad { get; set; }

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
}
