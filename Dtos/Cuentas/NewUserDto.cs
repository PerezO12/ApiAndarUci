using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class NewUserDto
    {
        public string id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? NombreUsuario { get; set;} 
        public IList<string> Rol { get; set; } = new List<string>();
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}