using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class EstudianteMappers
    {
        public static Estudiante UpdateEstudiante(this Estudiante estudianteExistente, Estudiante estudianteModelo)
        {
            estudianteExistente.UsuarioId = estudianteModelo.UsuarioId;
            estudianteExistente.CarreraId = estudianteModelo.CarreraId;
            estudianteExistente.FacultadId = estudianteModelo.FacultadId;
            estudianteExistente.Activo = estudianteModelo.Activo;

            return estudianteExistente;
        }
    }
}