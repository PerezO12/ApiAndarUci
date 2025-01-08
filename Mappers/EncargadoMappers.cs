using ApiUci.Dtos.Encargado;
using ApiUci.Models;

namespace ApiUci.Mappers
{
    public static class EncargadoMappers
    {
        public static Encargado UpdateEncargado(this Encargado encargadoExistente, EncargadoUpdateDto encargadoUpdateDto)
        {
            encargadoExistente.UsuarioId = encargadoUpdateDto.userId ?? encargadoExistente.UsuarioId;
            encargadoExistente.DepartamentoId = encargadoUpdateDto.DepartamentoId ?? encargadoExistente.DepartamentoId;
            encargadoExistente.LlavePublica = encargadoUpdateDto.LlavePublica ??  encargadoExistente.LlavePublica;
            encargadoExistente.Activo = encargadoUpdateDto.Activo ?? encargadoExistente.Activo;

            return encargadoExistente;
        }
        public static EncargadoDto toEncargadoDtoFromEncargado(this Encargado encargadoModel, IList<string>? roles = null)
        {
            return new EncargadoDto{
                    Id = encargadoModel.Id,
                    UsuarioId = encargadoModel.UsuarioId,
                    Departamento = encargadoModel.Departamento!.toDepartamentDto(),
                    Facultad = encargadoModel.Departamento!.Facultad!.toFacultadDto(),
                    NombreCompleto = encargadoModel.Usuario!.NombreCompleto,
                    CarnetIdentidad = encargadoModel.Usuario.CarnetIdentidad,
                    UserName = encargadoModel.Usuario.UserName,
                    Email = encargadoModel.Usuario.Email,
                    NumeroTelefono = encargadoModel.Usuario.PhoneNumber,
                    Roles = roles
            };
        }
    }
}