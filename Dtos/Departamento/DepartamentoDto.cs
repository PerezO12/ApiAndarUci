using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Departamento
{
    public class DepartamentoDto
    {
        public int Id { get; set; } //borar luego
        public string Nombre { get; set; } = null!;

        public int FacultadId { get; set; }
    }
}