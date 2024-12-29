using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Departamento
{
    public class PatchDepartamentoDto
    {
        public string? Nombre { get; set; }
        public int? FacultadId { get; set; }
    }
}