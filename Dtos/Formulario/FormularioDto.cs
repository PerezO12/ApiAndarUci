using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Formulario
{
    public class FormularioDto
    {
        //Datos del estudiante
        public string NombreCompleto { get; set; } = null!;
        public string? NombreUsuario { get; set; }
        public string CarnetIdentidad { get; set; } = null!;
        public string? Email { get; set; }
        public string? NumeroTelefono { get; set; }
        public string NombreCarrera { get; set; } = null!;
        public string NombreFacultad { get; set; } = null!;

        //Datos del encargado
        public string NombreDepartamento { get; set; } = null!;
        public bool firmado { get; set;} = false;
        public string NombreEncargado {get; set;}  = null!;
        //Datos del formulario
        public DateTime? FechaFirmado { get; set; }
        public DateTime Fechacreacion { get; set; }
        //datos del motivo del estudiante
        public string Motivo { get; set; } = null!;

    }
}