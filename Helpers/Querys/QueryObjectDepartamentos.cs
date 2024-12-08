using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Helpers.Querys
{
    public class QueryObjectDepartamentos
    {
        public string? Departamento { get; set; } = null; //para buscar por
        public string? Facultad { get; set; } = null;
        public int? FacultadId { get;set; } = null;
        public string OrdenarPor { get; set; } = "Fecha"; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar
    }
}