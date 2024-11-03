using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Usuarios
{
    public class UsuarioDto
    {   
        public string? Id { get; set; }
        public string? NombreCompleto { get; set; }
        public string? CarnetIdentidad { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
    }
}