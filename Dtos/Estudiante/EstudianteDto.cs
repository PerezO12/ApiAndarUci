using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Carrera;
using ApiUci.Dtos.Facultad;

namespace ApiUci.Dtos.Estudiante
{
public class EstudianteDto
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = null!; 
    public string NombreCompleto { get; set; } = null!; 
    public string CarnetIdentidad { get; set; } = null!; 
    public string? UserName { get; set; } = string.Empty; 
    public string? Email { get; set; } = string.Empty; 
    public string? NumeroTelefono { get; set; } = string.Empty;
    public CarreraDto Carrera { get; set; } = null!; 
    public FacultadDto Facultad { get; set; } = null!;
    public IList<string>? Roles { get; set; }
}
}