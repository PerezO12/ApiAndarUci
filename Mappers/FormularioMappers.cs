using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class FormularioMappers
    {
        public static Formulario toFormularioFromCreate(this CreateFormularioDto formularioDto, string usuarioId, int facultadId)
        {
            return new Formulario{
                UsuarioId = usuarioId,
                DepartamentoId = formularioDto.DepartamentoId,
                FacultadId = facultadId,
                Motivo = formularioDto.Motivo
            };
        }
    }
}