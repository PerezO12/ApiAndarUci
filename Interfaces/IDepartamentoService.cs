using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{   
    public interface IDepartamentoService
    {
        Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> GetAllAsync( QueryObjectDepartamentos query );
        Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> GetAllDepartamentoByEstudiante(string userId);
        Task<RespuestasGenerales<DepartamentoDto>> GetByIdAsync(int departamentoId);
        Task<RespuestasGenerales<DepartamentoDto>> CreateAsync(CreateDepartamentoDto departamentoDto);
        Task<RespuestasGenerales<DepartamentoDto>> UpdateAsync(int departamentoId, UpdateDepartamentoDto departamentoDto);
        Task<RespuestasGenerales<DepartamentoDto>> PatchAsync(int departamentoId, PatchDepartamentoDto departamentoDto);
        Task<RespuestasGenerales<DepartamentoDto>> DeleteAsync(int departamentoId);
        Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> DeleteAllDepartamentosByFacultad(int facultadId);
    }
}