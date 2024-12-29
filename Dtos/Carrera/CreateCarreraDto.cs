using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Dtos.Carrera
{
    public class CreateCarreraDto
    {
        [Required(ErrorMessage = "El nombre de la carrera es obligatorio.")]
        [MinLength(3, ErrorMessage = "Nombre no válido.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "La facultad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Facultad no válida")]
        public int FacultadId { get; set; }
    }
}