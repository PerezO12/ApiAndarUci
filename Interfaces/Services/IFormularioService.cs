using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos;
using ApiUci.Dtos.Formulario;
using ApiUci.Helpers.Querys;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IFormularioService
    {
        Task<RespuestasGenerales<FormularioDto?>> GetFormularioWithDetailsAsync(int id);
        Task<RespuestasGenerales<List<FormularioDto>>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query);
        Task<RespuestasGenerales<List<FormularioEstudianteDto>>> GetAllFormulariosEstudiantesAsync(string usuarioId, QueryObjectFormularioEstudiantes query);
        Task<RespuestasGenerales<List<FormularioEncargadoDto>>> GetAllFormulariosEncargadosAsync(string usuarioId, QueryObjectFormularioEncargado query);
        Task<RespuestasGenerales<FormularioEncargadoDto?>> GetFormEstudianteByIdForEncargadoAsync(string userId, int formularioId );
        Task<RespuestasGenerales<FormularioEstudianteDto>> CreateFormularioAsync(string userId, CreateFormularioDto formularioDto);
        Task<RespuestasGenerales<FormularioEstudianteDto>> UpdatePatchFormularioAsync(string userId, int id, UpdateFormularioDto formularioDto);
    
        Task<RespuestasGenerales<FormularioEncargadoDto>> FirmarFormularioAsync(string userId, int id, FormularioFirmarDto formularioDto);
        Task<RespuestasGenerales<FormularioEstudianteDto>> DeleteFormularioEstudianteAsync(string userId, int formularioId);
        Task<RespuestasGenerales<FormularioDto?>> DeleteFormularioAdmin(int formularioId);
    }
}