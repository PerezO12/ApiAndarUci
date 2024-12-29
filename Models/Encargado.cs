﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Encargado
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string UsuarioId { get; set; } = null!;

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Departamento no válida")]
    public int DepartamentoId { get; set; } 

    public byte[]? LlavePublica { get; set; }

    public bool Activo { get; set; }  = true;

    [ForeignKey("DepartamentoId")]
    public virtual Departamento? Departamento { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual AppUser? AppUser { get; set;} 
}
