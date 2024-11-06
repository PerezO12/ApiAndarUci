using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;

namespace MyApiUCI.Interfaces
{
    public interface IEstudianteService
    {
        public Task<List<EstudianteDto>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query);
    }
}