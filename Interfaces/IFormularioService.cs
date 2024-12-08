using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Formulario;
using ApiUCI.Helpers.Querys;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioService
    {
        Task<FormularioDto?> GetFormularioWithDetailsAsync(int id);
        Task<List<FormularioDto>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query);
        Task<List<FormularioEstudianteDto>> GetAllFormulariosEstudiantesAsync(string usuarioId, QueryObjectFormularioEstudiantes query);
        Task<List<FormularioEncargadoDto>> GetAllFormulariosEncargadosAsync(string usuarioId, QueryObjectFormularioEncargado query);
        Task<FormularioEncargadoDto?> GetFormEstudianteByIdForEncargadoAsync(string userId, int formularioId );
        Task<ResultadoDto> CreateFormularioAync(string userId, CreateFormularioDto formularioDto);
        Task<ResultadoDto> UpdatePatchFormularioAsync(string userId, int id, UpdateFormularioDto formularioDto);
    
        Task<ResultadoDto> FirmarFormularioAsync(string userId, int id, FormularioFirmarDto formularioDto);
        Task<ResultadoDto> DeleteFormularioEstudianteAsync(string userId, int formularioId);
        Task<ResultadoDto> DeleteFormularioAdmin(int formularioId);
    }
}