using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiUci.Models;

public partial class Estudiante
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string UsuarioId { get; set; } = null!;

    [Required(ErrorMessage = "La carrera es obligatoria.")]
    public int CarreraId { get; set; }

    [Required(ErrorMessage = "La facultad es obligatoria.")]
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
