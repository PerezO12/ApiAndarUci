using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Cuentas;
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
        public async Task<NewUserDto?> Login(LoginDto loginDto)
        {
            try{
                var user = await  _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == loginDto.Nombre.ToLower());
                if(user == null) return null;

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if(!result.Succeeded) return null;

                var roles = await _userManager.GetRolesAsync(user);
                if(roles == null || !roles.Any())
                {
                    throw new Exception("El usuario no tiene roles asignados.");
                }
                var token = await _tokenService.CreateTokenAsync(user);
                // Guarda el token en la base de datos
                await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);
                return new NewUserDto
                    {   
                        id = user.Id,
                        NombreCompleto = user.NombreCompleto,
                        NombreUsuario = user.UserName,
                        Email = user.Email,
                        Rol = roles[0],
                        Token = token
                    };
            }
            catch(Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        
        }

        public async Task<UserPerfilDto?> ObtenerPerfilAsync(string id)
        {
            var usuarioModel = await _userManager.FindByIdAsync(id);
            if(usuarioModel == null) return null; 
            var rol = await _userManager.GetRolesAsync(usuarioModel);
            if(rol == null) return null;    
            return new UserPerfilDto {
                id = id,
                NombreCompleto = usuarioModel.NombreCompleto,
                NombreUsuario = usuarioModel.UserName,
                Email = usuarioModel.Email,
                Rol = rol[0]
            };
        }

        public async Task<bool> VerifyUserPassword(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    
        public async Task<IdentityResult> CambiarPasswordAsync(AppUser user, CambiarPasswordDto cuentaDto)
        {
            try
            {
                return await _userManager.ChangePasswordAsync(user, cuentaDto.PasswordActual, cuentaDto.PasswordNueva );
            }
            catch(Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }
        public async Task<AppUser?> ExisteUsuario(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}