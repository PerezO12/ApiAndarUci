using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyApiUCI.Models
{
    public class AppUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = null!;
        public string CarnetIdentidad { get; set; } = null!;
        public bool Activo { get; set; } = true;
    }
}