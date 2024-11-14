    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ApiUCI.Dtos.Formulario;
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

            public static Formulario toPatchFormulario(this Formulario formularioModel, UpdateFormularioDto formularioDto)
            {
                formularioModel.FirmadoPor = formularioDto.FirmadoPor ?? formularioModel.FirmadoPor;
                formularioModel.DepartamentoId = formularioDto.DepartamentoId ?? formularioModel.DepartamentoId;
                formularioModel.FacultadId = formularioDto.FacultadId ?? formularioModel.FacultadId;
                formularioModel.Firmado = formularioDto.Firmado ?? formularioModel.Firmado;
                formularioModel.FirmaEncargado = formularioDto.FirmaEncargado ?? formularioModel.FirmaEncargado;
                formularioModel.FechaFirmado = formularioDto.FechaFirmado ?? formularioModel.FechaFirmado;
                formularioModel.Motivo = formularioDto.Motivo ?? formularioModel.Motivo;

                return formularioModel;
            }
        }
    }