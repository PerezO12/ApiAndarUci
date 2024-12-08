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
        public Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query);
        public Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id);
        public Task<EncargadoDto?> GetByUserIdWithUserId(string id);
        public Task<Encargado?> GetEncaradoByUserId(string userId);
        public Task<Encargado?> GetEncargadoByDepartamentoId(int departamentoId);
        public Task<EncargadoFirmaDto?> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto);
        public Task<EncargadoFirmaDto?> GenerarFirmaDigitalAsync(string userId);
        Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId);
    }
}