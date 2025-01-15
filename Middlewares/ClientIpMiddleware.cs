using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Middlewares
{
    public class ClientIpMiddleware
    {
    private readonly RequestDelegate _next;

    public ClientIpMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "IP no encontrada";
        Console.WriteLine("¨{{{{{{{{{{{{{{{{{{{{{{{{{}}}}}}}}}}}}}}}}}}}}}}}}}");
        Console.WriteLine(clientIp);
        context.Items["ClientIp"] = clientIp; 

        await _next(context);
    }
    }
}