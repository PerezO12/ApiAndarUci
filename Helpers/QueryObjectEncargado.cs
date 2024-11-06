using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApiUCI.Helpers
{
    public class QueryObjectEncargado
    {
        public string? Nombre { get; set; } = null; //para buscar por
        public string? UsuarioId { get; set; } = null;
//        public int? FacultadId { get; set; } = null; //para buscar por
        public int? DepartamentoId { get; set; } = null;
        public List<int> ListaId {get; set; }= new List<int>();
        public List<int> ListaDepartamentoId {get; set; }= new List<int>();
        public List<string> ListaUserId {get; set; }= new List<string>();
        public string? OrdernarPor { get; set; } = null; //Para ordenar
        public bool Descender { get; set; } = false; 
        public int NumeroPagina { get; set; } = 1; //Paginainicial por defecto 1
        public int TamañoPagina { get; set; } = 10; //Cantidad d elementos a retornar
    }
}