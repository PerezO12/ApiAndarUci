using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;

namespace MyApiUCI.Interfaces
{
    public interface IEncargadoService
    {
        public Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query);
        public Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id);
    }
}