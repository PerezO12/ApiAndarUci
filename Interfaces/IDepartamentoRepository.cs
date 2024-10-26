using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IDepartamentoRepository
    {
        public Task<List<Departamento>> GetAllAsync( QueryObject query );
        public Task<Departamento?> GetByIdAsync(int id);
        public Task<Departamento> CreateAsync(Departamento departamentoModel);
        public Task<Departamento?> UpdateAsync(int id, Departamento departdepartamentoModelamento);
        public Task<Departamento?> DeleteAsync(int id);
        public Task<Departamento?> PatchAsync(int id, PatchDepartamentoDto departamentoDto);
    }
}