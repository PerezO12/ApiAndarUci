using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IAuthService
    {
        Task<NewUserDto?> Login(LoginDto loginDto);
        Task<UserPerfilDto?> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(AppUser user, string password);
        Task<IdentityResult> CambiarPasswordAsync(AppUser user, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
    }
}