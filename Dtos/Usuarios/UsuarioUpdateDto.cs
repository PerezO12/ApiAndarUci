using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Usuarios
{
    public class UsuarioUpdateDto
    {
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
        public string? CarnetIdentidad { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
    }
}