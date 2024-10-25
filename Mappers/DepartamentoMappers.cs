using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class DepartamentoMappers
    {
        public static DepartamentoDto toDepartamentDto(this Departamento departamentoModel)
        {
            return new DepartamentoDto{
                Id = departamentoModel.Id, //Borrar
                Nombre = departamentoModel.Nombre,
                FacultadId = departamentoModel.FacultadId
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
                FacultadId = departamentoDto.FacultadId
            };
        }
    }
}