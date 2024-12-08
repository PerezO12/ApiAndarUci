using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IEncargadoRepository
    {
        Task<List<Encargado>> GetAllAsync(QueryObjectEncargado query);
        Task<Encargado?> GetByIdAsync(int id);
        Task<Encargado?> GetByUserIdAsync(string id);
        Task<Encargado?> GetEncargadoByDepartamentoId(int departamentoId);
        Task<Encargado?> GetEncargadoByUserIdAsync(string userId);
        Task<Encargado> CreateAsync(Encargado encargadoModel);
        Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoDto);
        Task<Encargado?> UpdateEncargadoByUserIdAsync(string id, EncargadoUpdateDto encargadoDto);
        Task<Encargado?> DeleteAsync(int id);
        Task<Encargado?> DeleteByUserIdAsync(string userId);
        Task<Encargado?> DeleteByDepartamentoIdAsync(int departamentoId);
        Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId);
    }
}