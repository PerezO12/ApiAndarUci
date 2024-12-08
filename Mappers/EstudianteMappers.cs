using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Estudiante;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class EstudianteMappers
    {
        public static Estudiante UpdateEstudiante(this Estudiante estudianteExistente, EstudianteUpdateDto estudianteUpdateDto)
        {
            estudianteExistente.UsuarioId = estudianteUpdateDto.userId ?? estudianteExistente.UsuarioId ;
            estudianteExistente.CarreraId = estudianteUpdateDto.CarreraId ?? estudianteExistente.CarreraId;
            estudianteExistente.FacultadId = estudianteUpdateDto.FacultadId ?? estudianteExistente.FacultadId;
            estudianteExistente.Activo = estudianteUpdateDto.Activo ?? estudianteExistente.Activo;

            return estudianteExistente;
        }
    }
}