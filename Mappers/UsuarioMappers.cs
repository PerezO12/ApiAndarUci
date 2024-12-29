using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Models;

namespace ApiUCI.Mappers
{
    public static class UsuarioMappers
    {
        public static UsuarioDto toUsuarioDto(this AppUser usuario, IList<string> roles)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                CarnetIdentidad = usuario.CarnetIdentidad,
                Activo = usuario.Activo,
                NombreUsuario = usuario.UserName!,
                Email = usuario.Email,
                NumeroTelefono = usuario.PhoneNumber,
                Roles = roles
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
                NombreUsuario = usuario.UserName!,
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
                NombreUsuario = usuario.UserName!,
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
                NombreUsuario = usuario.UserName!,
                Email = usuario.Email,
                Roles = roles,
                Token = token
            };
        }
        public static AppUser updateAppUserFromUsuarioWhiteRole(this AppUser usuario, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            usuario.NombreCompleto = usuarioUpdateDto.NombreCompleto ?? usuario.NombreCompleto;
            usuario.Activo = usuarioUpdateDto.Activo ?? usuario.Activo;
            usuario.CarnetIdentidad = usuarioUpdateDto.CarnetIdentidad ?? usuario.CarnetIdentidad;
            usuario.UserName = usuarioUpdateDto.NombreUsuario ?? usuario.UserName;
            usuario.Email = usuarioUpdateDto.Email ?? usuario.Email;
            usuario.PhoneNumber = usuarioUpdateDto.NumeroTelefono ?? usuario.PhoneNumber;

            return usuario;
        }
    }
}