using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFacultadRepository
    {
        public Task<List<Facultad>> GetAllAsync(QueryObjectFacultad query);
        public Task<Facultad?> GetByIdAsync(int id);
        public Task<Facultad> CreateAsync(Facultad facultadModel);
        public Task<Facultad?> UpdateAsync(int id, Facultad facultadModel);
            //Task<Facultad?> PatchFacultad(int id, Facultad facultad)
        public Task<Facultad?> DeleteAsync(int id);
        public Task<bool> FacultyExists(int id);

    }
}