using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Helpers
{
    public class QueryObjectFormulario
    {
        //buscar por
        public string? UsuarioId { get; set; } = null;
        public string? NombreEstudiante { get; set; } = null;
        public int? DepartamentoId { get;set; } = null;
        public string? DepartamentoNombre { get; set; }
        public List<int> ListaId {get; set; }= new List<int>();
        //Ordenar Por
        public string? OrdernarPor { get; set; } = null; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar 

    }
}