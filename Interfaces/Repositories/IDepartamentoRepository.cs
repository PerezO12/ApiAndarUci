using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Helpers.Querys;
using ApiUci.Dtos.Departamento;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IDepartamentoRepository
    {
        Task<List<Departamento>> GetAllAsync( QueryObjectDepartamentos query );
        Task<List<Departamento>> GetAllDepartamentosByFacultadId(int id);
        Task<Departamento?> GetByIdAsync(int id);
        Task<Departamento> CreateAsync(Departamento departamentoModel);
        Task<Departamento?> UpdateAsync(int id, Departamento departdepartamentoModelamento);
        Task<Departamento?> CambiarEncargado(int departamentoId, int? nuevoEncargado = null);
        Task<Departamento?> DeleteAsync(int id);
        Task<Departamento?> DeleteEncargadoByEncargadoID(int encargadoId);
        Task<List<Departamento>> DeleteAllDepartamentosByFacultad(int facultadId);//cuidado!!!
        Task<Departamento?> PatchAsync(int id, PatchDepartamentoDto departamentoDto);
        Task<bool> ExistDepartamento(int id);
        Task<bool> TieneEncargado(int id);
    }
}