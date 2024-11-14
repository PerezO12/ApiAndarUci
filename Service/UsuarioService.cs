using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using MyApiUCI.Repository;

namespace ApiUCI.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<AppUser> _usuarioManager;
        public UsuarioService(UserManager<AppUser> usuarioManager)
        {
            _usuarioManager = usuarioManager;
        }
        public async Task<IdentityResult> UpdateAsync(string id, UsuarioUpdateDto usuarioUpdateDto)
        {
            var usuario = await _usuarioManager.FindByIdAsync(id);
            if(usuario == null) return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado"});

            // Mapear los valores manualmente desde el DTO
            usuario.NombreCompleto = usuarioUpdateDto.NombreCompleto ?? usuario.NombreCompleto;
            usuario.Activo = usuarioUpdateDto.Activo ?? usuario.Activo;
            usuario.CarnetIdentidad = usuarioUpdateDto.CarnetIdentidad ?? usuario.CarnetIdentidad;
            usuario.UserName = usuarioUpdateDto.NombreUsuario ?? usuario.UserName;
            usuario.Email = usuarioUpdateDto.Email ?? usuario.Email;
            usuario.PhoneNumber = usuarioUpdateDto.NumeroTelefono ?? usuario.PhoneNumber;

            // Actualizar el usuario
            var result = await _usuarioManager.UpdateAsync(usuario);

            return IdentityResult.Success;
        }
    }
}