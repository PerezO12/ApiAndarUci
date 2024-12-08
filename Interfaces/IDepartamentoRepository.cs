using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IDepartamentoRepository
    {
        public Task<List<Departamento>> GetAllAsync( QueryObjectDepartamentos query );
        public Task<List<Departamento>> GetAllDepartamentosByFacultadId(int id);
        public Task<Departamento?> GetByIdAsync(int id);
        public Task<Departamento> CreateAsync(Departamento departamentoModel);
        public Task<Departamento?> UpdateAsync(int id, Departamento departdepartamentoModelamento);
        Task<Departamento?> CambiarEncargado(int departamentoId, int? nuevoEncargado = null);
        public Task<Departamento?> DeleteAsync(int id);
        public Task<Departamento?> PatchAsync(int id, PatchDepartamentoDto departamentoDto);
    }
}