using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using MyApiUCI.Dtos.Carrera;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface ICarreraService
    {
        Task<RespuestasGenerales<IEnumerable<CarreraDto>>> GetAllAsync(QueryObjectCarrera query);
        Task<RespuestasGenerales<CarreraDto>> GetByIdAsync( int carreraId ); 
        Task<RespuestasGenerales<CarreraDto>> CreateAsync( CreateCarreraDto carreraDto );
        Task<RespuestasGenerales<CarreraDto>> UpdateAsync( int carreraId, UpdateCarreraDto updateCarreraDto );
        Task<RespuestasGenerales<CarreraDto>> DeleteAsync( int carreraId );
        Task<RespuestasGenerales<CarreraDto>> PatchAsync( int carreraId, PatchCarreraDto carreraDto );

    }
}