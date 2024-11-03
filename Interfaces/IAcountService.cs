using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IAcountService
    {
        public Task<(IdentityResult, NewEstudianteDto?)> RegisterEstudiante(RegisterEstudianteDto registerDto);
        public Task<(IdentityResult, NewEncargadoDto?)> RegisterEncargado(RegisterEncargadoDto registerDto); 
        public Task<NewUserDto?> Login(LoginDto loginDto);
    }
}