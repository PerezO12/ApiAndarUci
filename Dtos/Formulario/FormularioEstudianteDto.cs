using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Formulario
{
    public class FormularioEstudianteDto
    {
        public int Id { get; set; }
        public string NombreEncargado {get; set;}  = null!;
        public string NombreDepartamento { get; set; } = null!;
        public string Motivo { get; set; } = null!;
        public bool Firmado { get; set;} = false;
        public DateTime? FechaFirmado { get; set; }
        public DateTime Fechacreacion { get; set; }
    }
}