using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Encargado
{
    public class EncargadoCambiarLlaveDto
    {
        [Required]
        public string Password { get; set; } = null! ;
        [Required]
        public string LlavePublica { get; set; } = null!;
    }
}