using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiUci.Filters
{
    public class LoggingFilter : IActionFilter
    {
        private readonly ILogger<LoggingFilter> _logger;
        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Acción finalizada: {ActionName} con resultado {Result}",
            context.ActionDescriptor.DisplayName,
            context.Result);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Ejecutando acción: {ActionName} con parámetros {Parameters}",
            context.ActionDescriptor.DisplayName,
            context.ActionArguments);
        }
    }
}