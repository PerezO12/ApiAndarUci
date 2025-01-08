
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Estudiante;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IEstudianteService
    {
        public Task<RespuestasGenerales<List<EstudianteDto>>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query);
        public Task<RespuestasGenerales<EstudianteDto?>> GetEstudianteWithByUserId(string id);
        public Task<RespuestasGenerales<Estudiante?>> GetEstudianteByUserId(string id);
        public Task<RespuestasGenerales<EstudianteDto?>> GetByIdWithDetailsAsync(int id);
        public Task<RespuestasGenerales<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto);
    }
}