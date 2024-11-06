using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace MyApiUCI.Helpers
{
    public class FormularioUsuarioDepartamento
    {
            public Formulario? Formulario { get; set; }
            public AppUser? Usuario { get; set; }
            public Departamento? Departamento { get; set; }
    }
}