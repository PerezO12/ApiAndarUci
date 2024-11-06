using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Formulario
{
    public class CreateFormularioDto
    {
        [Required]
        public int DepartamentoId { get; set; }

        [Required]
        [MaxLength(500)]
        [MinLength(5)]
        public required  string Motivo { get; set; }
    }
}