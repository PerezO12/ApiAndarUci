using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class RegistroAdministradorDto
    {
        public string NombreUsuario { get; set; } = null!;        
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public string PasswordAdmin { get; set; } = null!;
    }
}