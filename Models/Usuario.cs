using System;
using System.Collections.Generic;

namespace MyApiUCI.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string NombreApellidos { get; set; } = null!;

    public string NumeroIdentidad { get; set; } = null!;

    public string NumeroCarnet { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int RolId { get; set; }

    public bool? Activo { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public virtual Encargado? Encargado { get; set; }

    public virtual Estudiante? Estudiante { get; set; }

    public virtual Rol Rol { get; set; } = null!;
}
