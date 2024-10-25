using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApiUCI.Models;

public partial class Estudiante
{
    [Key]
    public int UsuarioId { get; set; }

    public string Carrera { get; set; } = null!;

    public int? FacultadId { get; set; }

    public bool? Activo { get; set; }

    public virtual Facultad? Facultad { get; set; }

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();

    public virtual Usuario Usuario { get; set; } = null!;
}
