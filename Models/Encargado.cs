using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApiUCI.Models;

public partial class Encargado
{
    [Key]
    public int UsuarioId { get; set; }

    public int? DepartamentoId { get; set; }

    public byte[] FirmaDigital { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual Departamento? Departamento { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
