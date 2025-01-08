using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUci.Helpers.Querys
{
    public class QueryObjectFacultad
    {
        public string? Nombre { get; set; } = null;

        public string OrdernarPor { get; set; } = "Fecha"; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar
    }
}