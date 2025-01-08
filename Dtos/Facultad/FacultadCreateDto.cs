using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos.Facultad
{
    public class FacultadCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}