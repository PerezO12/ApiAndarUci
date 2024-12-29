using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Formulario
{
    public class CreateFormularioDto
    {
        public int DepartamentoId { get; set; }
        public required  string Motivo { get; set; }
    }
}