using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Encargado
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UsuarioId { get; set; } = null!;

    [Required]
    public int DepartamentoId { get; set; } 

    public byte[]? LlavePublica { get; set; }

    public bool Activo { get; set; }  = true;

    [ForeignKey("DepartamentoId")]
    public virtual Departamento? Departamento { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual AppUser? AppUser { get; set;} 
}
