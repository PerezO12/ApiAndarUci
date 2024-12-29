using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Departamento
{
    public class CreateDepartamentoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "La facultad es requerida")]
        public int FacultadId { get; set; }
    }
}