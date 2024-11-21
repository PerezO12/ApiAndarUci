using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Formulario
{
    public class UpdateFormularioDto
    {
        [Required]
        [MaxLength(800)]
        [MinLength(5)]
        public string Motivo { get; set; } = null!;

    }
}