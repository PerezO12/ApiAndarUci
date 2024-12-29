using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class NewEstudianteDto
    {
        public string Id { get; set; } = null!;
        public string NombreUsuario { get; set;} = null!; 
        public string CarnetIdentidad { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public string? Carrera { get; set; }
        public string? Facultad {get; set;}
        public string? Token { get; set; }
        public bool Activo { get; set;}
    }
}