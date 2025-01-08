using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Dtos
{
    public class SensitiveActionRequestDto
    {
        [Required]
        public string Password { get; set; } = null!;
    }
}