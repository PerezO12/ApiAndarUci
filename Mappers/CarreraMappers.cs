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