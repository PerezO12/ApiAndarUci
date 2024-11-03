using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class UsuarioMappers
    {
        public static UsuarioDto toUsuarioDto(this AppUser usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                CarnetIdentidad = usuario.CarnetIdentidad,
                UserName = usuario.UserName,
                Email = usuario.Email,
                NumeroTelefono = usuario.PhoneNumber,
            };
        }
    }
}