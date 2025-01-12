using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Extensions;
using ApiUci.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiUci.Mappers;
using ApiUci.Models;

namespace ApiUci.Service
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
        public async Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == loginDto.UserName.ToLower());
                
                if (user == null)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Usuario/Contraseña","Usuario o contraseña Incorrectos");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                
                if (!result.Succeeded)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Usuario/Contraseña","Usuario o contraseña Incorrectos");

                var roles = await _userManager.GetRolesAsync(user);
                
                if (roles == null || !roles.Any())
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Roles", "No se pudo obtener el perfil del usuario.", "Unauthorized");

                var token = await _tokenService.CreateTokenAsync(user);

                await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);

                var newPerfilUserDto = user.toUserPerfilDto(roles, token);

                return RespuestasGenerales<UserPerfilDto>.SuccessResponse(newPerfilUserDto, "Sesión iniciada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string usuarioId)
        {
            try
            {
                var usuarioModel = await _userManager.FindByIdAsync(usuarioId);
                if (usuarioModel == null)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("UsuarioId", "El usuario no existe.");

                var roles = await _userManager.GetRolesAsync(usuarioModel);
                if (roles == null || !roles.Any())
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Roles", "No se pudo obtener el perfil del usuario.", "Unauthorized");

                var userPerfilDto = usuarioModel.toUserPerfilDto(roles);

                return RespuestasGenerales<UserPerfilDto>.SuccessResponse(userPerfilDto, "Perfil obtenido exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<bool> VerifyUserPassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
            
        public async Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if(usuario == null)
                    return RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe");
                var resultado = await _userManager.ChangePasswordAsync(usuario, cuentaDto.PasswordActual, cuentaDto.PasswordNueva);

                if (!resultado.Succeeded)
                {
                    var errors = ErrorBuilder.ParseIdentityErrors(resultado.Errors);
                    return RespuestasGenerales<bool>.ErrorResponseController(errors, "Error al cambiar la contraseña.");
                }

                return RespuestasGenerales<bool>.SuccessResponse(true, "Contraseña cambiada exitosamente.");
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

        public Task<RespuestasGenerales<bool>> LogoutAsync(string userId)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                if (user == null)
                    return Task.FromResult(RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe"));

                var result = _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "AccessToken").Result;

                if (!result.Succeeded)
                    return Task.FromResult(RespuestasGenerales<bool>.ErrorResponseService("Token", "Error al cerrar sesión"));

                return Task.FromResult(RespuestasGenerales<bool>.SuccessResponse(true, "Sesión cerrada exitosamente"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}