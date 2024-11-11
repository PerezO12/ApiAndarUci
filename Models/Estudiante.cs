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
    [ForeignKey("AppUser")]
    public string UsuarioId { get; set; } = null!;

    [Required]
    public int CarreraId { get; set; }
    [Required]
    public int FacultadId { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;
    public DateTime? FechaBaja { get; set;} = null;

    public virtual Facultad Facultad { get; set; }  = null!;
    public virtual Carrera Carrera { get; set; }  = null!;

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
    
    public virtual AppUser AppUser { get; set; } = null!;
}
