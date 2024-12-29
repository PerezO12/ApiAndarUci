using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ApiUCI.Dtos.Cuentas
{
    public class RegisterEstudianteDto
    {
        public string? NombreUsuario { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public int CarreraId { get; set; }
        public int FacultadId { get; set; }
        
    }
}