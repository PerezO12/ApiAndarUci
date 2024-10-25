using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiUCI.Models;

public partial class Facultad
{
    [Key]
    [Column("id")]
    public int Id { get; private set;}

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("fechacreacion")]
    public DateTime FechaCreacion { get; private set;}  = DateTime.Now.ToUniversalTime();
    [Column("activo")]
    public bool Activo { get; set; } = true;

    public virtual ICollection<Carrera> Carreras { get; set; } = new List<Carrera>();

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
