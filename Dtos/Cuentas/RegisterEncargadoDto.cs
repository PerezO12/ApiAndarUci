using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Cuentas
{
    public class RegisterEncargadoDto
    {
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(10, ErrorMessage = "No es un nombre valido")]
        public string NombreCompleto { get; set; } = null!;

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El Carnet de Identidad debe tener exactamente 11 caracteres.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El Carnet de Identidad solo debe contener números.")]
        public string CarnetIdentidad { get; set; } = null!;

        [Required]
        public int DepartamentoId { get; set; }

        [Required]
        public string FirmaDigital  { get; set; } = null!;

    }
}