using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Formulario
{
    public class FormularioFirmadoDto
    {
        public int FormularioId { get; set; }
        public int EstudianteId { get; set; }
        public int EncargadoId { get; set; }
        public string NombreEstudiante { get; set; }  = null!;
        public string NombreEncargado { get; set; }  = null!;
        public string NombreDepartamento { get; set; }  = null!;
        public DateTime Fechacreacion { get; set; } 
        public string Motivo{  get; set; } = null!;
    }
}