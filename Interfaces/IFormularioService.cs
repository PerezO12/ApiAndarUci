using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Formulario;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioService
    {
        Task<FormularioDto?> GetFormularioWithDetailsAsync(int id);
        Task<List<FormularioDto>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query);
        Task<List<FormularioEstudianteDto>> GetAllFormulariosEstudiantesAsync(string usuarioId);
        Task<List<FormularioEncargadoDto>> GetAllFormulariosEncargadosAsync(string usuarioId);
        //Task<List<Formulario>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query);
        //Task<FormularioDto> GetFormularioWithDetailsAsync()
        Task<ResultadoDto> CreateFormularioAync(string userId, CreateFormularioDto formularioDto);
        Task<ResultadoDto> UpdatePatchFormularioAsync(string userId, int id, UpdateFormularioDto formularioDto);
    
        Task<ResultadoDto> FirmarFormularioAsync(string userId, int id, FormularioFirmarDto formularioDto);
    }
}