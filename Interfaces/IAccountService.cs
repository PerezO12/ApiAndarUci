using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IAccountService
    {
        public Task<(IdentityResult, NewEstudianteDto?)> RegisterEstudianteAsync(RegisterEstudianteDto registerDto);
        public Task<(IdentityResult, NewEncargadoDto?)> RegisterEncargadoAsync(RegisterEncargadoDto registerDto);
        public Task<(IdentityResult, NewAdminDto?)> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto); 
        public Task<NewUserDto?> Login(LoginDto loginDto);
        public Task<IdentityResult> CambiarPasswordAsync(string userId,  CambiarPasswordDto cuentaDto);
        //public Task<IdentityResult> RestablecerPasswordAsync(string UserName,  CambiarPasswordDto cuentaDto);
        public Task<UserPerfilDto?> ObtenerPerfilAsync(string id);
    }
}