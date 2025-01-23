using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiUci.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Ocurrió un error no manejado");

            context.Result = new ObjectResult(new
            {
                Details = context.Exception.Message,
                Message = "Ha ocurrido un error. Por favor, contacte al soporte."
            })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }
}