using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Cuentas
{
    public class NewEncargadoDto
    {
        public string Id { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public string? UserName { get; set;} = null!;
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? Departamento { get; set; }
        public bool Activo { get; set;}
        public string? Token { get; set; }
    }
}