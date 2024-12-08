using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class EncargadoMappers
    {
        public static Encargado UpdateEncargado(this Encargado encargadoExistente, EncargadoUpdateDto encargadoUpdateDto)
        {
            encargadoExistente.UsuarioId = encargadoUpdateDto.userId ?? encargadoExistente.UsuarioId;
            encargadoExistente.DepartamentoId = encargadoUpdateDto.DepartamentoId ?? encargadoExistente.DepartamentoId;
            encargadoExistente.LlavePublica = encargadoUpdateDto.LlavePublica ??  encargadoExistente.LlavePublica;
            encargadoExistente.Activo = encargadoUpdateDto.Activo ?? encargadoExistente.Activo;

            return encargadoExistente;
        }
    }
}