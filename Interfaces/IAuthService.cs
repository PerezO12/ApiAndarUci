using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IAuthService
    {
        Task<RespuestasServicios<NewUserDto?>> Login(LoginDto loginDto);
        Task<RespuestasServicios<UserPerfilDto?>> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(string userId, string password);
        Task<RespuestasServicios<bool>> CambiarPasswordAsync(AppUser user, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
    }
}