using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Dtos.Facultad;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class FacultadMappers
    {
        public static FacultadDto toFacultadDto(this Facultad facultadModel)
        {
            return new FacultadDto
            {
                Id = facultadModel.Id, //Borrar
                Nombre = facultadModel.Nombre
            };
        }
        public static Facultad toFacultadFromCreate(this FacultadCreateDto facultadDto)
        {
            return new Facultad{
                Nombre = facultadDto.Nombre
            };
        }
        public static Facultad toFacultadFromUpdate(this FacultadUpdateDto facultaDto)
        {
            return new Facultad{
                Nombre = facultaDto.Nombre
            };
        }
    }
}