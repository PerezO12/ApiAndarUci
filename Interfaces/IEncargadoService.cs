using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Encargado;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IEncargadoService
    {
        Task<RespuestasGenerales<List<EncargadoDto>>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query);
        Task<RespuestasGenerales<EncargadoDto?>> GetByIdEncargadoWithDetailsAsync(int id);
        Task<RespuestasGenerales<EncargadoDto?>> GetByUserIdWithUserId(string id);
        Task<Encargado?> GetEncaradoByUserId(string userId);
        Task<Encargado?> GetEncargadoByDepartamentoIdAsync(int departamentoId);
        Task<RespuestasGenerales<EncargadoFirmaDto?>> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto);
        Task<RespuestasGenerales<EncargadoFirmaDto?>> GenerarFirmaDigitalAsync(string userId, EncargadoGenerarLLaveDto encargadoDto);
        Task<Encargado?> DeleteAsync(int id);
        Task<Encargado?> DeleteByUserIdAsync(string userId);
        Task<Encargado?> DeleteEncargadoByDepartamentoIdAsync(int departamentoId, bool borrarDepartamento = true);
        Task<Encargado> CreateAsync(Encargado encargadoModel);
        Task<RespuestasGenerales<NewEncargadoDto>> RegisterEncargadoAsync(RegisterEncargadoDto registerDto);
        Task<Encargado?> UpdateEncargadoByUserIdAsync(string id, EncargadoUpdateDto encargadoDto);
        Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoDto);
        Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId);
    }
}
