using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Departamento
{
    public class UpdateDepartamentoDto
    {
        public string Nombre { get; set; } = null!;
        public int FacultadId { get; set; }
    }
}