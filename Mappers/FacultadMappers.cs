using ApiUCI.Dtos.Facultad;
using ApiUCI.Models;

namespace ApiUCI.Mappers
{
    public static class FacultadMappers
    {
        public static FacultadDto toFacultadDto(this Facultad facultadModel)
        {
            return new FacultadDto
            {
                Id = facultadModel.Id, //Borrar
                Nombre = facultadModel.Nombre,
                FechaCreacion = facultadModel.FechaCreacion
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