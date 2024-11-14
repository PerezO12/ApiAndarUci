using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioRepository
    {
        Task<IEnumerable<Formulario>> GetAllAsync(QueryObjectFormulario query);
        Task<Formulario> CreateAsync(Formulario formulario);
        Task<Formulario?> GetByIdAsync(int id);
        Task<Formulario?> UpadatePatchAsync(UpdateFormularioDto formulario, int id);
        Task<Formulario?> DeleteAsync(int id);
    }
}
