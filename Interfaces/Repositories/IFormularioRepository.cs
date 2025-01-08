using ApiUci.Dtos.Formulario;
using ApiUci.Helpers.Querys;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
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
