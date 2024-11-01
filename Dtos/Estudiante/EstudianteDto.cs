using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Estudiante
{
    public class EstudianteDto
    {
        public string NombreCompleto { get; set; }
        public string CarnetIdentidad { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set;}
        public string? Carrera { get; set; }
        public string? Facultad { get; set; }
    }
}