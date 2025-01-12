
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Estudiante;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IEstudianteService
    {
        Task<RespuestasGenerales<List<EstudianteDto>>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query);
        Task<RespuestasGenerales<EstudianteDto?>> GetEstudianteWithByUserId(string id);
        Task<RespuestasGenerales<Estudiante?>> GetEstudianteByUserId(string id);
        Task<RespuestasGenerales<EstudianteDto?>> GetByIdWithDetailsAsync(int id);
        Task<RespuestasGenerales<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto);

        Task<bool> ComprobarBajaEstudiante(int estudianteId);
    }
}