using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class UserPerfilDto
    {
        public string Id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? NombreUsuario { get; set;}
        public IList<string> Roles { get; set; } = new List<string>();
        public string? Email { get; set; }
        public string? Token {get; set;}
    }
}