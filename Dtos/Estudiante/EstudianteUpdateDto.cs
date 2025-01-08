using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Estudiante
{
    public class EstudianteUpdateDto
    {
        public string? userId { get; set; }
        public int? CarreraId { get; set; }
        public int? FacultadId { get; set; }
        public bool? Activo { get; set; }
    }
}