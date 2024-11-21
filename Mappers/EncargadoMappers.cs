using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace MyApiUCI.Mappers
{
    public static class EncargadoMappers
    {
        public static Encargado UpdateEncargado(this Encargado encargadoExistente, Encargado encargadoModelo)
        {
            encargadoExistente.UsuarioId = encargadoModelo.UsuarioId;
            encargadoExistente.DepartamentoId = encargadoModelo.DepartamentoId;
            encargadoExistente.LlavePublica = encargadoModelo.LlavePublica;
            encargadoExistente.Activo = encargadoModelo.Activo;

            return encargadoExistente;
        }
    }
}