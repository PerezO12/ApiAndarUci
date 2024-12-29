using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ApiUCI.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [MinLength(8, ErrorMessage = "Nombre no válido.")]
        public string NombreCompleto { get; set; } = null!;
        [Required(ErrorMessage = "El carnet es obligatorio.")]
        public string CarnetIdentidad { get; set; } = null!;
        public bool Activo { get; set; } = true;
    }
}