using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Carrera
{
    public class CarreraDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Facultad {get; set;} = null!;
        public DateTime? FechaCreacion { get; set; }
    }
}