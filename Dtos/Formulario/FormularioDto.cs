using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Formulario
{
    public class FormularioDto
    {
        //Datos del estudiante
        public int id { get; set; }
        public string NombreEstudiante { get; set; } = null!;
        public string? UserNameEstudiante { get; set; }
        public string CarnetIdentidadEstudiante { get; set; } = null!;
        public string? EmailEstudiante { get; set; }
        public string NombreCarrera { get; set; } = null!;
        public string? NombreFacultad { get; set; }
        //Datos del encargado
        public string NombreDepartamento { get; set; } = null!;
        public bool Firmado { get; set;} = false;
        public string NombreEncargado {get; set;}  = null!;
        //Datos del formulario
        public DateTime? FechaFirmado { get; set; }
        public DateTime Fechacreacion { get; set; }
        //datos del motivo del estudiante
        public string Motivo { get; set; } = null!;

    }
}