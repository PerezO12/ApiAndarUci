using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFacultadRepository
    {
        public Task<List<Facultad>> GetAllAsync();
        public Task<Facultad?> GetByIdAsync(int id);
        public Task<Facultad> CreateAsync(Facultad facultadModel);
        public Task<Facultad?> UpdateAsync(int id, Facultad facultadModel);
            //Task<Facultad?> PatchFacultad(int id, Facultad facultad)
        public Task<Facultad?> DeleteAsync(int id);
        public Task<bool> FacultyExists(int id);

    }
}