using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class UserPerfilDto
    {
        public string id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? NombreUsuario { get; set;}
        public IList<string>? Rol { get; set; } = null!;
        public string? Email { get; set; }
    }
}