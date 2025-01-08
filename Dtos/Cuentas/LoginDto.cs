using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Cuentas
{
    public class LoginDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}