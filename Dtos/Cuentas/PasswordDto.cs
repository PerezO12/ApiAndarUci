using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get;set; } = null!;
    }
}