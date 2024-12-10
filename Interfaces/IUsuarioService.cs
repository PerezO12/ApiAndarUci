using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Usuarios;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IUsuarioService
    {
        Task<IdentityResult> UpdateAsync(string id, UsuarioWhiteRolUpdateDto ususarioDto);
        Task<ResultadoDto> DeleteUserYRolAsync(string usuarioId, string adminId);
        Task<List<UsuarioDto>> GetAllAsync(QueryObjectUsuario query);
        Task<UsuarioDto?> GetByIdAsync(string id);
        Task<AppUser?> DeleteAsync(string id);
    }
}