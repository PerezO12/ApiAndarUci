using System.Security.Claims;

namespace ApiUci.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims
                .FirstOrDefault(x => x.Type == "https://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value ?? string.Empty;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            // Utiliza el claim "sub" o "UsuarioId" según tu definición en el token
            var userId = user.FindFirstValue("sub") ?? user.FindFirstValue("UsuarioId");
            
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Token no válido o usuario no encontrado");
                
            return userId;
        }
        public static bool IsTokenTemporal(this ClaimsPrincipal user)
        {
            // Utiliza el claim "sub" o "UsuarioId" según tu definición en el token
            var tokenTemp =  user.FindFirstValue("temp_token");
            
            if (string.IsNullOrEmpty(tokenTemp) || tokenTemp != "true")
                return false;
                
            return true;
        }



        public static string GetUserId1(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims
                .FirstOrDefault(x => x.Type == "UsuarioId")?.Value ?? string.Empty;
        }
    }
}