using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Estudiante
{
    public class EstudianteDto
    {
        public string UsuarioId { get; set; }
        public int CarreraId { get; set; }
        public int FacultadId { get; set; }
    }
}