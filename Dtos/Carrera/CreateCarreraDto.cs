using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Carrera
{
    public class CreateCarreraDto
    {
        public string Nombre { get; set; } = null!;
        public int FacultadId { get; set; }
    }
}