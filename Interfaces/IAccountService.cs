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
        public Task<RespuestasServicios<NewEncargadoDto>> RegisterEncargadoAsync(RegisterEncargadoDto registerDto);
        public Task<RespuestasServicios<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto);
        public Task<RespuestasServicios<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto); 
        
        
    }
}