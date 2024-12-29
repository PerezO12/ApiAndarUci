using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IUsuarioService
    {
        Task<RespuestasGenerales<UsuarioDto>> UpdateAsync(string id, UsuarioWhiteRolUpdateDto ususarioDto);
        Task<RespuestasGenerales<UsuarioDto>> DeleteUserYRolAsync(string usuarioId);
        Task<RespuestasGenerales<List<UsuarioDto>>> GetAllAsync(QueryObjectUsuario query);
        Task<RespuestasGenerales<UsuarioDto?>> GetByIdAsync(string id);
        Task<RespuestasGenerales<UsuarioDto?>> DeleteAsync(string id);
        Task<RespuestasGenerales<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto); 
    }
}