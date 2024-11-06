using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiUCI.Models;

public partial class Formulario
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string UsuarioId { get; set; }

    public string? FirmadoPor { get; set; }

    public int DepartamentoId { get; set; }

    public int FacultadId { get; set; }

    public bool Firmado { get; set; } = false;

    public byte[]? FirmaEncargado { get; set; }

    public DateTime? FechaFirmado { get; set; }
    
    public DateTime Fechacreacion { get; set; } = DateTime.Now.ToUniversalTime();

    [Required]
    [MaxLength(500)]
    [MinLength(5)]
    public string Motivo { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public virtual Estudiante? Estudiante { get; set; }

    [ForeignKey("FacultadId")]
    public virtual Facultad? Facultad { get; set; }

    [ForeignKey("DepartamentoId")]
    public virtual Departamento? Departamento { get; set; }
    
    [ForeignKey("FirmadoPor")]
    public virtual AppUser? UsuarioFirmadoPor { get; set; }


}
