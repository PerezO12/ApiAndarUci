using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Departamento
{
    public class CreateDepartamentoDto
    {
        public string Nombre { get; set; } = null!;
        public int FacultadId { get; set; }
    }
}