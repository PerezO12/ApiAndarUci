using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Estudiante
{
public class EstudianteDto
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty; 
    public string NombreCompleto { get; set; } = string.Empty; 
    public string CarnetIdentidad { get; set; } = string.Empty; 
    public string? NombreUsuario { get; set; } = string.Empty; 
    public string? Email { get; set; } = string.Empty; 
    public string? NumeroTelefono { get; set; } = string.Empty;
    public string NombreCarrera { get; set; } = string.Empty; 
    public string NombreFacultad { get; set; } = string.Empty;
}
}