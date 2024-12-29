using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Usuarios
{
    public class UsuarioWhiteRolUpdateDto
    {
        public string? NombreCompleto { get; set; }
        public bool? Activo { get; set; }
        public string? CarnetIdentidad { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
        public List<string> Roles { get; set; } = new List<string>();
        public int DepartamentoId { get; set; } = 0;
        public int FacultadId { get; set; } = 0;
        public int CarreraId { get; set; } = 0;
        public string PasswordAdmin { get; set; } = null!;
    }
}