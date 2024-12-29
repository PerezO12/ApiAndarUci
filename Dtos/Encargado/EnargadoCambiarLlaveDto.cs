using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Encargado
{
    public class EncargadoCambiarLlaveDto
    {
        public string Password { get; set; } = null! ;
        public string LlavePublica { get; set; } = null!;
    }
}