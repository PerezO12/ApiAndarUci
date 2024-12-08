using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioRepository
    {
        Task<List<FormularioDto>> GetAllAsync(QueryObjectFormulario query);
        Task<List<FormularioEstudianteDto>> GetAllFormulariosByEstudiante(string userId, QueryObjectFormularioEstudiantes query);
        Task<List<FormularioEncargadoDto>> GetAllFormulariosByEncargado(string userId, QueryObjectFormularioEncargado query);
        Task<Formulario> CreateAsync(Formulario formulario);
        Task<Formulario?> GetByIdAsync(int id);
        Task<FormularioEncargadoDto?> GetFormEstudianteByIdForEncargadoAsync(int encargadoId, int formularioId );
        Task<Formulario?> UpadatePatchAsync(int id, UpdateFormularioDto formulario);
        Task<Formulario?> UpdateAsync(int id, Formulario formulario);
        Task<Formulario?> DeleteAsync(int id);
        Task<Formulario?> FormByEstudianteDepartamentoAsync(int estudianteId, int DepartamentoId);

    }
}
