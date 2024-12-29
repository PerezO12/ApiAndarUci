using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class CambiarPasswordDto
    {
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string PasswordActual { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string PasswordNueva { get; set; } = null!;
    }
}