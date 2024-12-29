
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Estudiante;
using ApiUCI.Helpers;
using ApiUCI.Models;

namespace ApiUCI.Interfaces
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