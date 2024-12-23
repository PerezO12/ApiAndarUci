using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IEstudianteService
    {
        public Task<RespuestasServicios<List<EstudianteDto>>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query);
        public Task<RespuestasServicios<EstudianteDto?>> GetEstudianteWithByUserId(string id);
        public Task<RespuestasServicios<Estudiante?>> GetEstudianteByUserId(string id);
        public Task<RespuestasServicios<EstudianteDto?>> GetByIdWithDetailsAsync(int id);
        public Task<RespuestasServicios<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto);
    }
}