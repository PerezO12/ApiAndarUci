using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class NewAdminDto
    {
        public string Id { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public string? NombreUsuario { get; set;} = null!;
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool Activo { get; set;}
    }
}