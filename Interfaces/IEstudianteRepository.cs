using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IEstudianteRepository
    {
        public Task<List<Estudiante>> GetAllAsync(QueryObject query);
        public Task<Estudiante?> GetByIdAsync(int id);
        public Task<Estudiante> CreateAsync(Estudiante estudianteModel);
        public Task<Estudiante?> UpdateAsync(int id, Estudiante estudianteModel);
        public Task<Estudiante?> DeleteAsync(int id);
    }
}