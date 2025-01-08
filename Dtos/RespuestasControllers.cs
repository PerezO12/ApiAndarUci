using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Extensions;
using ApiUci.Utilities;
using ApiUci.Dtos.Usuarios;

namespace ApiUci.Dtos
{
    public class RespuestasGenerales<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? Campo { get; set; }
        public string? Message { get; set; }
        public string ActionResult { get; set; } = null!;
        //public ErrorType ErrorType { get; private set; } = ErrorType.None;

        public static RespuestasGenerales<T> SuccessResponse(T data, string message = "Operación realizada exitosamente")
        {
            return new RespuestasGenerales<T> { Success = true, Data = data, Message = message, ActionResult = "Ok" };
        }
        public static RespuestasGenerales<T> ErrorResponseController(Dictionary<string, string[]> errors, string message = "Error en la solicitud", string resultado = "BadRequest")
        {
            return new RespuestasGenerales<T> { Success = false, Errors = errors, Message = message, ActionResult = resultado };
        }
        public static RespuestasGenerales<T> ErrorResponseService( string campo = "General", string message = "Error en la solicitud", string actionResult = "BadRequest")
        {
            var error = ErrorBuilder.Build(campo, message);
            return new RespuestasGenerales<T> { Success = false, Errors = error, ActionResult = actionResult };
        }

        internal static RespuestasGenerales<UsuarioDto> ErrorResponseService(UsuarioDto usuarioDto, string v)
        {
            throw new NotImplementedException();
        }
    }
}

