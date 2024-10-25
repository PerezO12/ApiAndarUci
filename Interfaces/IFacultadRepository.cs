using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFacultadRepository
    {
        Task<List<Facultad>> GetAllAsync();
        Task<Facultad?> GetByIdAsync(int id);
        Task<Facultad> CreatedAsync(Facultad facultadModel);
        Task<Facultad?> UpdateAsync(int id, Facultad facultadModel);
        //Task<Facultad?> PatchFacultad(int id, Facultad facultad)
        Task<Facultad?> DeleteAsync(int id);

    }
}