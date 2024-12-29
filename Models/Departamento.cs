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
    [MinLength(3, ErrorMessage = "El nombre no es válido.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La facultad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Facultad no válida")]
    public int FacultadId { get; set; }

    [ForeignKey("Encargado")]
    [Range(1, int.MaxValue, ErrorMessage = "Encargado no válido")]
    public int? EncargadoId { get; set; }

    public DateTime? Fechacreacion { get; set; } = DateTime.Now.ToUniversalTime();
    
    public bool Activo { get; set; } = true;

    public virtual Encargado? Encargado { get; set; }

    public virtual Facultad? Facultad { get; set; }

    public virtual ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
}
