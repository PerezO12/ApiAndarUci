using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class LoginDto
    {
        [Required]
        public string Nombre { get; set; } = null!;
        [Required]
        public string Password { get; set;} = null!;
    }
}