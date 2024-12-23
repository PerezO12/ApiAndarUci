using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace ApiUCI.Service
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        public async Task<RespuestasServicios<NewUserDto?>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == loginDto.Nombre.ToLower());

                // Verificar si el usuario existe
                if (user == null)
                {
                    var errors = ErrorBuilder.Build("Username", "El nombre de usuario no existe.");
                    return RespuestasServicios<NewUserDto?>.ErrorResponse(errors);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                // Verificar si las credenciales son correctas
                if (!result.Succeeded)
                {
                    var errors = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                    return RespuestasServicios<NewUserDto?>.ErrorResponse(errors);
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    var errors = ErrorBuilder.Build("Roles", "El usuario no tiene roles asignados.");
                    return RespuestasServicios<NewUserDto?>.ErrorResponse(errors);
                }

                var token = await _tokenService.CreateTokenAsync(user);

                await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);

                var newUserDto = new NewUserDto
                {
                    id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    NombreUsuario = user.UserName,
                    Email = user.Email,
                    Rol = roles[0], // Suponiendo que el usuario solo tiene un rol
                    Token = token
                };

                return RespuestasServicios<NewUserDto?>.SuccessResponse(newUserDto, "Inicio de sesión exitoso.");
            }
            catch (Exception ex)
            {
                // Manejo de error general
                var errors = ErrorBuilder.Build("Exception", $"Error inesperado: {ex.Message}");
                return RespuestasServicios<NewUserDto?>.ErrorResponse(errors);
            }
        }


        public async Task<RespuestasServicios<UserPerfilDto?>> ObtenerPerfilAsync(string id)
        {
            try
            {
                var usuarioModel = await _userManager.FindByIdAsync(id);
                if (usuarioModel == null)
                {
                    var errors = ErrorBuilder.Build("UserId", "El usuario no existe");
                    return RespuestasServicios<UserPerfilDto?>.ErrorResponse(errors, "No se pudo obtener el perfil del usuario.");
                }

                var rol = await _userManager.GetRolesAsync(usuarioModel);
                if (rol == null || !rol.Any())
                {
                    var errors = ErrorBuilder.Build("Roles", "El usuario no tiene roles asignados.");
                    return RespuestasServicios<UserPerfilDto?>.ErrorResponse(errors, "No se pudo obtener el perfil del usuario.");
                }

                var userPerfilDto = new UserPerfilDto
                {
                    id = id,
                    NombreCompleto = usuarioModel.NombreCompleto,
                    NombreUsuario = usuarioModel.UserName,
                    Email = usuarioModel.Email,
                    Rol = rol[0]
                };

                return RespuestasServicios<UserPerfilDto?>.SuccessResponse(userPerfilDto, "Perfil obtenido exitosamente");
            }
            catch (Exception ex)
            {
                // Manejo de excepciones y retorno de error general
                var errors = ErrorBuilder.Build("Exception", $"Error al obtener el perfil del usuario: {ex.Message}");
                return RespuestasServicios<UserPerfilDto?>.ErrorResponse(errors, "Error al obtener el perfil del usuario.");
            }
        }


        public async Task<bool> VerifyUserPassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
            
        public async Task<RespuestasServicios<bool>> CambiarPasswordAsync(AppUser user, CambiarPasswordDto cuentaDto)
        {
            try
            {
                var resultado = await _userManager.ChangePasswordAsync(user, cuentaDto.PasswordActual, cuentaDto.PasswordNueva);

                if (!resultado.Succeeded)
                {
                    var errors = ErrorBuilder.ParseIdentityErrors(resultado.Errors);
                    return RespuestasServicios<bool>.ErrorResponse(errors, "Error al cambiar la contraseña.");
                }

                return RespuestasServicios<bool>.SuccessResponse(true, "Contraseña cambiada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<AppUser?> ExisteUsuario(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }


    }
}