using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class NewEstudianteDto
    {
        public string? UserName { get; set;} 
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Carrera { get; set; }
        public string? Facultad {get; set;}
        public string? Token { get; set; }
    }
}