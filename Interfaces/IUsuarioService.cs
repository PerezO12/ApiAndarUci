using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Usuarios;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IUsuarioService
    {
        Task<IdentityResult> UpdateAsync(string id, UsuarioUpdateDto ususarioDto);
    }
}