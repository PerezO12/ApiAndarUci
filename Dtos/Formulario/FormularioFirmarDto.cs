using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Formulario
{
    public class FormularioFirmarDto
    {
        [Required]
        public string LlavePrivada { get; set; } = null!;
    }
}