using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Interfaces.Services;

namespace ApiUCI.Middlewares
{
    public class IpBlockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpBlockMiddleware> _logger;

        public IpBlockMiddleware(RequestDelegate next, ILogger<IpBlockMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, IIpBlockService ipBlockService)
        {
            try
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                if (ipAddress != null && await ipBlockService.IsBlockedAsync(ipAddress))
                {
                    _logger.LogWarning("Bloqueando solicitud desde la IP: {IpAddress}", ipAddress);

                    //todo: cambiar respuesta
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Tu IP está bloqueada.");
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el middleware de bloqueo de IP.");
                throw;
            }
        }
    }
}