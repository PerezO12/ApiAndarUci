using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiUCI.Filters
{
    public class EstandarResponseFilter : IActionFilter
    {
         public void OnActionExecuting(ActionExecutingContext context)
        {
            // No necesitamos hacer nada antes de la ejecución de la acción
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                return;
            }

            if (context.Result is ObjectResult objectResult)
            {
                var statusCode = objectResult.StatusCode ?? 200;

                // Crear una respuesta envolviendo el resultado original
                var response = new RespuestasGenerales<object>
                {
                    Success = statusCode >= 200 && statusCode < 300,
                    Data = statusCode >= 200 && statusCode < 300 ? objectResult.Value : null,
                    Errors = statusCode >= 400 ? ParseErrors(objectResult.Value!) : null,
                    Message = statusCode >= 200 && statusCode < 300 ? "Operación exitosa" : "Error en la solicitud"
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = statusCode
                };
            }
        }

        private Dictionary<string, string[]> ParseErrors(object value)
        {
            // Manejar el formato de errores según lo que retornen tus controladores
            if (value is Dictionary<string, string[]> errors)
            {
                return errors;
            }

            return new Dictionary<string, string[]>
            {
                { "General", new[] { value?.ToString() ?? "Error desconocido" } }
            };
        }
    }
}