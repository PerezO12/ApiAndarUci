using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Usuarios;
using ApiUci.Models;

namespace ApiUci.Mappers
{
    public static class UsuarioMappers
    {
        public static UsuarioDto toUsuarioDto(this AppUser usuario, IEnumerable<string>? roles = null)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto ?? string.Empty,
                CarnetIdentidad = usuario.CarnetIdentidad ?? string.Empty, 
                Activo = usuario.Activo,
                UserName = usuario.UserName,
                Email = usuario.Email,
                NumeroTelefono = usuario.PhoneNumber,
                Roles = roles ?? new List<string>()
            };
        }
        public static UsuarioDto toUsuarioDtoBorrar(this AppUser usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                CarnetIdentidad = usuario.CarnetIdentidad,
                Activo = usuario.Activo,
                UserName = usuario.UserName,
                Email = usuario.Email,
                NumeroTelefono = usuario.PhoneNumber
            };
        }
        public static NewAdminDto toAdminDto(this AppUser usuario, IList<string> roles)
        {
            return new NewAdminDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                CarnetIdentidad = usuario.CarnetIdentidad,
                Activo = usuario.Activo,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Roles = roles
            };
        }

        public static UserPerfilDto toUserPerfilDto(this AppUser usuario, IList<string> roles, string? token = null)
        {
            return new UserPerfilDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Roles = roles,
                Token = token
            };
        }
        public static AppUser updateAppUserFromUsuarioWhiteRole(this AppUser usuario, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            usuario.NombreCompleto = usuarioUpdateDto.NombreCompleto ?? usuario.NombreCompleto;
            usuario.Activo = usuarioUpdateDto.Activo;
            usuario.CarnetIdentidad = usuarioUpdateDto.CarnetIdentidad ?? usuario.CarnetIdentidad;
            usuario.UserName = usuarioUpdateDto.UserName ?? usuario.UserName;
            usuario.Email = usuarioUpdateDto.Email ?? usuario.Email;
            usuario.PhoneNumber = usuarioUpdateDto.NumeroTelefono ?? usuario.PhoneNumber;

            return usuario;
        }
    }
}