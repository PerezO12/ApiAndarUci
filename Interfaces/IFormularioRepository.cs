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
        Task<List<Formulario>> GetAllAsync(QueryObjectFormulario query);
        Task<List<FormularioEstudianteDto>> GetAllFormulariosByEstudiante(string userId);
        Task<List<FormularioEncargadoDto>> GetAllFormulariosByEncargado(string userId);
        Task<Formulario> CreateAsync(Formulario formulario);
        Task<Formulario?> GetByIdAsync(int id);
        Task<Formulario?> UpadatePatchAsync(int id, UpdateFormularioDto formulario);
        Task<Formulario?> UpdateAsync(int id, Formulario formulario);
        Task<Formulario?> DeleteAsync(int id);
        Task<Formulario?> FormByEstudianteDepartamentoAsync(int estudianteId, int DepartamentoId);

    }
}
