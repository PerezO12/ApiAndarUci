using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Utilities;

namespace ApiUCI.Dtos
{
    public class RespuestasServicios<T>
    {
        public T? Data { get; private set; }
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public ErrorType ErrorType { get; private set; } = ErrorType.None;

        public static RespuestasServicios<T> SuccessResult(T data) 
            => new RespuestasServicios<T> { Data = data, Success = true };
        public static RespuestasServicios<T> FailureResult(string error, ErrorType errorType)
            => new RespuestasServicios<T> { Success = false, ErrorMessage = error, ErrorType = errorType };
    }
}