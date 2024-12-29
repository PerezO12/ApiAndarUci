using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class NewUserDto
    {
        public string id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? NombreUsuario { get; set;} 
        public string Rol { get; set; } = null!;
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}