    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiUCI.Dtos.Formulario;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Dtos.Formulario;
    using MyApiUCI.Models;

    namespace MyApiUCI.Mappers
    {
        public static class FormularioMappers
        {
            public static Formulario toFormularioFromCreate(this CreateFormularioDto formularioDto, Estudiante estudiante, Encargado encargado)
            {
                return new Formulario{
                    EstudianteId = estudiante.Id,
                    EncargadoId = encargado.Id,
                    DepartamentoId = formularioDto.DepartamentoId,
                    Motivo = formularioDto.Motivo
                };
            }

            public static Formulario toPatchFormulario(this Formulario formularioModel, UpdateFormularioDto formularioDto)
            {
/*                 formularioModel.FirmadoPorId = formularioDto.FirmadoPor ??  formularioModel.FirmadoPorId; */
                //formularioModel.DepartamentoId = formularioDto.DepartamentoId ?? formularioModel.DepartamentoId;
/*                 formularioModel.FacultadId = formularioDto.FacultadId ?? formularioModel.FacultadId;
                formularioModel.Firmado = formularioDto.Firmado ?? formularioModel.Firmado;
                formularioModel.FirmaEncargado = formularioDto.FirmaEncargado ?? formularioModel.FirmaEncargado;
                formularioModel.FechaFirmado = formularioDto.FechaFirmado ?? formularioModel.FechaFirmado; */
                formularioModel.Motivo = formularioDto.Motivo ?? formularioModel.Motivo;

                return formularioModel;
            }
        }
    }