using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiUci.Models;

public partial class Encargado
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string UsuarioId { get; set; } = null!;

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    public int DepartamentoId { get; set; }

    public byte[]? LlavePublica { get; set; }

    public bool Activo { get; set; } = true;

    public virtual Departamento? Departamento { get; set; }

    public virtual AppUser? Usuario { get; set;} 
}
