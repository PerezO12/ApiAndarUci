using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Carrera;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface ICarreraRepository
    {
        public Task<List<Carrera>> GetAllAsync();
        public Task<Carrera?> GetByIdAsync( int id ); 
        public Task<Carrera> CreateAsync( Carrera carreraModel );
        public Task<Carrera?> UpdateAsync( int id, Carrera carreraModel );
        public Task<Carrera?> PatchAsync( int id, PatchCarreraDto carreraDto );
        public Task<Carrera?> DeleteAsync( int id);
    }
}