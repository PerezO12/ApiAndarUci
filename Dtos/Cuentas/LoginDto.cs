using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [MinLength(4, ErrorMessage = "No es un nombre válido.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set;} = null!;
    }
}