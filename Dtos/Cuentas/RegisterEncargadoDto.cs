using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Cuentas
{
    public class RegisterEncargadoDto
    {
        public string UserName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string CarnetIdentidad { get; set; } = string.Empty;

        public int DepartamentoId { get; set; }

    }
}