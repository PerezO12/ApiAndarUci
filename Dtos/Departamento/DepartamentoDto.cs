using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Departamento
{
    public class DepartamentoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Facultad { get; set; }
        public string? EncargadoNombre { get; set; }
        public int? EncargadoId {get; set;}
        public DateTime? FechaCreacion { get; set; }
    }
}