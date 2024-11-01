using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class NewEncargadoDto
    {
        public string UserName { get; set;} 
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public string Departamento { get; set; }
        public string Token { get; set; }
    }
}