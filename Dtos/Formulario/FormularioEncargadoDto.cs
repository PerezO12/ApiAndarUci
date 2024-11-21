using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Formulario
{
    public class FormularioEncargadoDto
    {
        public int Id { get; set; }
        public string NombreCompletoEstudiante { get; set; } = null!;
        public string NombreCarrera { get; set; } = null!;
        public string Motivo { get; set; } = null!;
        public DateTime Fechacreacion { get; set; }
    }
}