using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Dtos.Facultad;

namespace ApiUCI.Interfaces
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