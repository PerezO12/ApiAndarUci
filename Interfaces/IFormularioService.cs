using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IFormularioService
    {
        Task<FormularioDto?> GetFormularioWithDetailsAsync(int id);
        Task<List<FormularioDto>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query);
        //Task<FormularioDto> GetFormularioWithDetailsAsync()
    }
}