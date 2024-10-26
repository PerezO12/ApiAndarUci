using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Carrera
{
    public class PatchCarreraDto
    {
        public string? Nombre { get; set; }
        public int? FacultadId { get; set; }
    }
}