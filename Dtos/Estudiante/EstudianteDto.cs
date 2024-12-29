using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Estudiante
{
public class EstudianteDto
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = null!; 
    public string NombreCompleto { get; set; } = null!; 
    public string CarnetIdentidad { get; set; } = null!; 
    public string? NombreUsuario { get; set; } = string.Empty; 
    public string? Email { get; set; } = string.Empty; 
    public string? NumeroTelefono { get; set; } = string.Empty;
    public string NombreCarrera { get; set; } = null!; 
    public string NombreFacultad { get; set; } = null!;
}
}