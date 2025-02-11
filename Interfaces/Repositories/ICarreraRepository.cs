using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Carrera;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface ICarreraRepository
    {
        public Task<List<Carrera>> GetAllAsync(QueryObjectCarrera query);
        public Task<Carrera?> GetByIdAsync( int id ); 
        public Task<Carrera> CreateAsync( Carrera carreraModel );
        public Task<Carrera?> UpdateAsync( int id, Carrera carreraModel );
        public Task<Carrera?> PatchAsync( int id, PatchCarreraDto carreraDto );
        public Task<Carrera?> DeleteAsync( int id);
        public Task<bool> ExisteCarrera(int id);
    }
}