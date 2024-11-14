using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Dtos.Formulario
{
    public class UpdateFormularioDto
    {
        public string? FirmadoPor { get; set; }

        public int? DepartamentoId { get; set; }

        public int? FacultadId { get; set; }

        public bool? Firmado { get; set; }

        public byte[]? FirmaEncargado { get; set; }

        public DateTime? FechaFirmado { get; set; }
    

        [MaxLength(500)]
        [MinLength(5)]
        public string? Motivo { get; set; }

    }
}