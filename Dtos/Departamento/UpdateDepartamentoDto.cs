using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Departamento
{
    public class UpdateDepartamentoDto
    {
        public string Nombre { get; set; } = null!;
        public int FacultadId { get; set; }
        public int EncargadoId { get; set; }
    }
}