using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos
{
    public class ResultadoDto
    {
        public string msg { get; set; } = null!;
        public string TipoError { get; set; } = null!;
        public bool Error { get; set; }
    }
}