using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Carrera
{
    public class PatchCarreraDto
    {
        public string? Nombre { get; set; }
        public int? FacultadId { get; set; }
    }
}