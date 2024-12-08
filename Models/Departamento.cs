using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Departamento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required]
    public int FacultadId { get; set; }

    public DateTime? Fechacreacion { get; set; } = DateTime.Now.ToUniversalTime();
    
    public bool Activo { get; set; } = true;

    public virtual ICollection<Encargado> Encargados { get; set; } = new List<Encargado>();

    public virtual Facultad? Facultad { get; set; }

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
}
