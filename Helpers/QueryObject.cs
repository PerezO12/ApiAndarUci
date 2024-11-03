using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Helpers
{
    public class QueryObject
    {
        public string? Nombre { get; set; } = null; //para buscar por
        public int? FacultadId { get; set; } = null; //para buscar por
        public int? CarreraId { get; set; } = null;
        public List<int> ListaId {get; set; }= new List<int>();
        public string? OrdernarPor { get; set; } = null; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar
    }
}