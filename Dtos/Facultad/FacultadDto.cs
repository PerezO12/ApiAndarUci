using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Facultad
{
    public class FacultadDto
    {
        public int Id { get; set; } //borrar
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set;}
    }
}