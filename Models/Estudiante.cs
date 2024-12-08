using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Estudiante
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UsuarioId { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la carrera debe ser positivo.")]
    public int CarreraId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la facultad debe ser positivo.")]  
    public int FacultadId { get; set; }

    public bool Activo { get; set; } = true;
    public DateTime? FechaBaja { get; set;} = null;

    [ForeignKey("FacultadId")]
    public virtual Facultad? Facultad { get; set; }
    [ForeignKey("CarreraId")]
    public virtual Carrera? Carrera { get; set; }

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
    [ForeignKey("UsuarioId")]
    public virtual AppUser? AppUser { get; set; }
}
