using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Cuentas
{
    public class CambiarPasswordDto
    {
        [Required]
        public string PasswordActual { get; set; } = null!;
        [Required]
        public string PasswordNueva { get; set; } = null!;
    }
}