using ApiUci.Contracts.V1;
using ApiUci.Extensions;
using Microsoft.AspNetCore.Identity;
using ApiUci.Models;

namespace ApiUci.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // Lista de rutas públicas, como la de login
        private static readonly string[] PublicRoutes = new string[]
        {
            $"/{ApiRoutes.Account.RutaGenaral}/{ApiRoutes.Account.Login}", // Ruta de login
        };

        public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var isTokenTemporal = context.User.IsTokenTemporal(); //Ver si el token es temporal
            if (isTokenTemporal && path.StartsWith($"/{ApiRoutes.Account.RutaGenaral}/{ApiRoutes.Account.Validar2Fa}", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            //Console.WriteLine("Rutas públicas: " + string.Join(", ", PublicRoutes));
            //Console.WriteLine("Path: " + path);
            // Excluir las rutas públicas
            if (string.IsNullOrEmpty(path) ||
                    PublicRoutes.Contains(path, StringComparer.OrdinalIgnoreCase) ||
                    path.StartsWith("/images", StringComparison.OrdinalIgnoreCase))
            {
                //Console.WriteLine("Entre: " + string.Join(", ", PublicRoutes));
                await _next(context);
                return;
            }
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var usuarioId = context.User.GetUserId();//TODO:VERIFICAR

            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Falta token o Token Inválido\"}");
                return;
            }

            var isValid = await IsTokenValid(usuarioId, token);

            if (!isValid)
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Inválido o Token expirado\"}");
                return;
            }

            await _next(context);
        }

        private async Task<bool> IsTokenValid(string userId, string token)
        {
            // Crear un alcance para obtener UserManager
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var user = await userManager.FindByIdAsync(userId);

                if (user == null) return false;

                var storedToken = await userManager.GetAuthenticationTokenAsync(user, "JWT", "AccessToken");
                
                if (storedToken != token)
                {
                    return false;
                }

                //TODO: Aquí podrías agregar la validación del tiempo de expiración del token si es necesario
                // Por ejemplo, verificando la fecha de expiración del token JWT (esto depende de tu implementación)

                return true;
            }
        }
    }
}
