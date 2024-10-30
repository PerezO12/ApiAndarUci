using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Encargado
{
    [Key]
    public int Id { get; set; }

   [ForeignKey("AppUser")]
    public string UsuarioId { get; set; } = null!;
    public int? DepartamentoId { get; set; }

    public byte[] FirmaDigital { get; set; } = null!;

    [Column("activo")]
    public bool Activo { get; set; }  = true;

    public virtual Departamento? Departamento { get; set; }

    public virtual AppUser AppUser { get; set;} = null!;
}
