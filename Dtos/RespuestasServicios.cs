using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Utilities;

namespace ApiUCI.Dtos
{
    public class RespuestasServicios<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? Message { get; set; }
        //public ErrorType ErrorType { get; private set; } = ErrorType.None;

        public static RespuestasServicios<T> SuccessResponse(T data, string message = "")
        {
            return new RespuestasServicios<T> { Success = true, Data = data, Message = message };
        }

        public static RespuestasServicios<T> ErrorResponse(Dictionary<string, string[]> errors, string message = "Error en la solicitud")
        {
            return new RespuestasServicios<T> { Success = false, Errors = errors, Message = message };
        }
    }
}

