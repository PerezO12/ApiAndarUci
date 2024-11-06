using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Estudiante
{
public class EstudianteDto
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty; 
    public string CarnetIdentidad { get; set; } = string.Empty; 
    public string UserName { get; set; } = string.Empty; 
    public string UsuarioId { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
    public string NumeroTelefono { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty; 
    public string Facultad { get; set; } = string.Empty;
}
}