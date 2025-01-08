using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiUci.Models;

public partial class Formulario
{
    [Key]
    public int Id { get; set; }    
    [Required(ErrorMessage = "El estudiante es obligatorio.")]
    public int EstudianteId { get; set; }
    [Required(ErrorMessage = "El encargado es obligatorio.")]
    public int EncargadoId { get; set; }

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    public int DepartamentoId { get; set; }
    public bool Firmado { get; set; } = false;

    public byte[]? FirmaEncargado { get; set; }
    public byte[]? HashDocumento { get; set; }

    public DateTime? FechaFirmado { get; set; }
    
    public DateTime Fechacreacion { get; set; } = DateTime.Now.ToUniversalTime();

    [Required]
    [MaxLength(800)]
    [MinLength(5)]
    public string Motivo { get; set; } = null!;

    public bool Activo { get; set; } = true;

    [ForeignKey("EstudianteId")]
    public virtual Estudiante? Estudiante { get; set; }

    [ForeignKey("EncargadoId")]
    public virtual Encargado? Encargado { get; set; }
    [ForeignKey("DepartamentoId")]
    public virtual Departamento? Departamento { get; set; }
    

}
