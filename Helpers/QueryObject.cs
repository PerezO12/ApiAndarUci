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
        public string? SortBy { get; set; } = null; //Para ordenar
        public bool IsDescending { get; set; } = false; 
        public int PageNumber { get; set; } = 1; //Paginainicial por defecto 1
        public int PageSize { get; set; } = 10; //Cantidad d elementos a retornar
    }
}