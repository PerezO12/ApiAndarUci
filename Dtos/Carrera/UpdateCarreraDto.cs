using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Carrera
{
    public class UpdateCarreraDto
    {
        public string Nombre { get; set; } = null!;
        public int FacultadId { get; set; }
    }
}