using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Estudiante;
using MyApiUCI.Dtos.Estudiante;
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

        public static EstudianteDto toEstudianteDto(this Estudiante estudiante)
        {
            return new EstudianteDto{
                Id = estudiante.Id,
                UsuarioId = estudiante.UsuarioId,
                NombreCompleto = estudiante.AppUser!.NombreCompleto,
                CarnetIdentidad = estudiante.AppUser.CarnetIdentidad,
                NombreUsuario = estudiante.AppUser.UserName,
                Email = estudiante.AppUser.Email,
                NumeroTelefono = estudiante.AppUser.PhoneNumber,
                NombreCarrera = estudiante.Carrera!.Nombre,
                NombreFacultad = estudiante.Facultad!.Nombre
            };
        }
    }
}