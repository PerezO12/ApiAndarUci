
using ApiUci.Dtos;
using ApiUci.Dtos.Carrera;
using ApiUci.Helpers;

namespace ApiUci.Interfaces
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