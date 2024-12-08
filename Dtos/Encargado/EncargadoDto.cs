using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Encargado
{
    public class EncargadoDto
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty; 
        public string NombreCompleto { get; set; } = string.Empty; 
        public string CarnetIdentidad { get; set; } = string.Empty; 
        public string? NombreUsuario { get; set; } = string.Empty; 
        public string? Email { get; set; } = string.Empty; 
        public string? NumeroTelefono { get; set; } = string.Empty;
        public string DepartamentoNombre { get; set; } = string.Empty;
        public string? FacultadNombre { get; set; }
        public int? DepartamentoId {get; set;} 
        //ver si agrego la firma digital o no
    }
}