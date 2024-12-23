using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Helpers
{
    public class QueryObjectUsuario
    {
        //BUSQUEDAS
        public bool SoloActivos {get; set;} = true;
        public string? Nombre { get; set; } = null; //para buscar por
        public string? NombreUsuario { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? CarnetIdentidad { get; set; } = null;
        //ORDENAR
        public string? OrdenarPor { get; set; } = null; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar 
    }
}