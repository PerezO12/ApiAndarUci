using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Carrera
{
    public class CarreraDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Facultad {get; set;}
        public DateTime? FechaCreacion { get; set; }
    }
}