using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IEncargadoRepository
    {
        Task<List<Encargado>> GetAllAsync(QueryObjectEncargado query);
        Task<Encargado?> GetByIdAsync(int id);
        Task<Encargado> CreateAsync(Encargado encargadoModel);
        Task<Encargado?> UpdateAsync(int id, Encargado encargadoModel);
        Task<Encargado?> DeleteAsync(int id);
    }
}