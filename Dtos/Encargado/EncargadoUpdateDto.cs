using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Encargado
{
    public class EncargadoUpdateDto
    {
        public string? userId { get; set; }
        public int? DepartamentoId { get; set; }
        public byte[]? LlavePublica { get; set; }
        public bool? Activo { get; set; }
    }
}