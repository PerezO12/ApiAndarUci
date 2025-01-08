using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Departamento;
using ApiUci.Dtos.Facultad;

namespace ApiUci.Dtos.Encargado
{
    public class EncargadoDto
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty; 
        public string NombreCompleto { get; set; } = string.Empty; 
        public string CarnetIdentidad { get; set; } = string.Empty; 
        public string? UserName { get; set; } = string.Empty; 
        public string? Email { get; set; } = string.Empty; 
        public string? NumeroTelefono { get; set; } = string.Empty;
        public DepartamentoDto Departamento { get; set; } = null!;
        public FacultadDto Facultad { get; set; } = null!;
        public string? FacultadNombre { get; set; }
        public IList<string>? Roles { get; set; }
        //ver si agrego la firma digital o no
    }
}