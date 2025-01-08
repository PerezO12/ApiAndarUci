
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Helpers.Querys;
using ApiUci.Dtos.Facultad;

namespace ApiUci.Interfaces
{
    public interface IFacultadService
    {
        Task<RespuestasGenerales<IEnumerable<FacultadDto>>> GetAllAsync(QueryObjectFacultad query);
        Task<RespuestasGenerales<FacultadDto?>> GetByIdAsync(int facultadId);
        Task<RespuestasGenerales<FacultadDto>> CreateAsync(FacultadCreateDto facultadDto);
        Task<RespuestasGenerales<FacultadDto?>> UpdateAsync(int facultadId, FacultadUpdateDto facultadDto);
        Task<RespuestasGenerales<FacultadDto?>> DeleteAsync(int facultadId);
    }
}