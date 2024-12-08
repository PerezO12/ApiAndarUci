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
    public interface IEncargadoService
    {
        Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query);
        Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id);
        Task<EncargadoDto?> GetByUserIdWithUserId(string id);
        Task<Encargado?> GetEncaradoByUserId(string userId);
        Task<Encargado?> GetEncargadoByDepartamentoIdAsync(int departamentoId);
        Task<EncargadoFirmaDto?> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto);
        Task<EncargadoFirmaDto?> GenerarFirmaDigitalAsync(string userId);
        Task<Encargado?> DeleteAsync(int id);
        Task<Encargado?> DeleteByUserIdAsync(string userId);
        Task<Encargado?> DeleteEncargadoByDepartamentoIdAsync(int departamentoId, bool borrarDepartamento = true);
        Task<Encargado> CreateAsync(Encargado encargadoModel);
        Task<Encargado?> UpdateEncargadoByUserIdAsync(string id, EncargadoUpdateDto encargadoDto);
        Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoDto);
        Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId);
    }
}