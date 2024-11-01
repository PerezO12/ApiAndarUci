using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Carrera;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class CarreraMappers
    {

        public static CarreraDto toCarreraDto(this Carrera carrera)
        {
            return new CarreraDto
            {
                Id = carrera.Id,
                Nombre = carrera.Nombre,
                FacultadId = carrera.FacultadId
            };
        }
        public static Carrera toCarreraFromUpdate(this UpdateCarreraDto carreraDto)
        {
            return new Carrera
            {
                Nombre = carreraDto.Nombre,
                FacultadId = carreraDto.FacultadId
            };
        }
        public static Carrera toCarreraFromCreate(this CreateCarreraDto carreraDto)
        {
            return new Carrera
            {
                Nombre = carreraDto.Nombre,
                FacultadId = carreraDto.FacultadId
            };
        }

        public static Carrera toPatchingCarrera(this Carrera carreraModel, PatchCarreraDto carreraDto)
        {
            
            if (!string.IsNullOrWhiteSpace(carreraDto.Nombre))
            {
                carreraModel.Nombre = carreraDto.Nombre;
            }
            if (carreraDto.FacultadId.HasValue)
            {
                carreraModel.FacultadId = carreraDto.FacultadId.Value;
            }
            return carreraModel;
        }
    }
}