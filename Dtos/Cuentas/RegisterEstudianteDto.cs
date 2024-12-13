using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace MyApiUCI.Dtos.Cuentas
{
    public class RegisterEstudianteDto
    {
        [Required]
        [MinLength(4, ErrorMessage = "No es un nombre válido")]
        public string? nombreUsuario { get; set; }
        
        [Required]
        [EmailAddress(ErrorMessage = "No es un correo válido")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "La contraseña es muy corta")]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(10, ErrorMessage = "No es un nombre válido")]
        [MaxLength(100, ErrorMessage = "No es un nombre válido")]
        public string NombreCompleto { get; set; } = null!;

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El carné de identidaddebe tener exactamente 11 números.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El carné de identidadsolo debe contener números.")]
        public string CarnetIdentidad { get; set; } = null!;

        [Required]
        public int CarreraId { get; set; }

        [Required]
        public int FacultadId { get; set; }
        
        //[Required (ErrorMessage= "La contraseña es requerida")]
        //public string PasswordAdmin { get; set; } = null!;

    }
}