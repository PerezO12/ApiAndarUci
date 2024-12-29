using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Usuarios
{
    public class UsuarioDto
    {   
        public string Id { get; set; } = null!;
        public bool Activo { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
        public IList<string> Roles { get; set; } = new List<string>();
    }
}