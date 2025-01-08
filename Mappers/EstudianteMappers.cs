using ApiUci.Dtos.Estudiante;
using ApiUci.Models;

namespace ApiUci.Mappers
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

        public static EstudianteDto toEstudianteDto(this Estudiante estudiante, IList<string>? roles = null)
        {
            return new EstudianteDto{
                Id = estudiante.Id,
                UsuarioId = estudiante.UsuarioId,
                NombreCompleto = estudiante.AppUser!.NombreCompleto,
                CarnetIdentidad = estudiante.AppUser.CarnetIdentidad,
                UserName = estudiante.AppUser.UserName,
                Email = estudiante.AppUser.Email,
                NumeroTelefono = estudiante.AppUser.PhoneNumber,
                Carrera = estudiante.Carrera!.toCarreraDto(),
                Facultad = estudiante.Facultad!.toFacultadDto(),
                Roles = roles,
            };
        }
    }
}