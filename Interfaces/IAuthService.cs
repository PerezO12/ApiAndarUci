using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IAuthService
    {
        Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto);
        Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(string userId, string password);
        Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
    }
}