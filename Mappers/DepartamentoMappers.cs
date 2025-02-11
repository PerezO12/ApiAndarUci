using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Departamento;
using ApiUci.Models;

namespace ApiUci.Mappers
{
    public static class DepartamentoMappers
    {
        public static DepartamentoDto toDepartamentDto(this Departamento departamentoModel)
        {
            return new DepartamentoDto{
                Id = departamentoModel.Id, //Borrar
                Nombre = departamentoModel.Nombre,
                Facultad = departamentoModel.Facultad?.Nombre,
                EncargadoNombre = departamentoModel.Encargado?.Usuario?.NombreCompleto ?? "",
                EncargadoId = departamentoModel.EncargadoId,
                FechaCreacion = departamentoModel?.Fechacreacion
            };
        }
        public static Departamento toDepartamentoFromCreate(this CreateDepartamentoDto departamentoDto)
        {
            return new Departamento{
                Nombre = departamentoDto.Nombre,
                FacultadId = departamentoDto.FacultadId
            };
        }
        public static Departamento toDepartamentoFromUpdate(this UpdateDepartamentoDto departamentoDto)
        {
            return new Departamento{
                Nombre = departamentoDto.Nombre,
                FacultadId = departamentoDto.FacultadId,
                //EncargadoId = departamentoDto.EncargadoId
            };
        }
        public static Departamento toPatchingDepartamento(this Departamento departamento, PatchDepartamentoDto departamentoDto)
        {
            if (!string.IsNullOrWhiteSpace(departamentoDto.Nombre))
            {
                departamento.Nombre = departamentoDto.Nombre;
            }
            if (departamentoDto.FacultadId.HasValue)
            {
                departamento.FacultadId = departamentoDto.FacultadId.Value;
            }
/*             if (departamentoDto.EncargadoId.HasValue)
            {
                departamento.EncargadoId = departamentoDto.EncargadoId.Value;
            } */
            return departamento;
        }
    }
}