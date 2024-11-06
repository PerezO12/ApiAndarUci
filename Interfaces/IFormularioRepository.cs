using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioRepository
    {
        Task<IEnumerable<Formulario>> GetAllAsync(QueryObjectFormulario query);
        Task<Formulario> CreateAsync(Formulario formulario);
        Task<Formulario?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Formulario formulario);
        Task<bool> DeleteAsync(int id);
    }
}
