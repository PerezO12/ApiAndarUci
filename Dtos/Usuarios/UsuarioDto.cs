using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Usuarios
{
    public class UsuarioDto
    {   
        public string Id { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string CarnetIdentidad { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
        public IEnumerable<string>? Roles { get; set; } = new List<string>();
    }
}