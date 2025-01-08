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
            /* return user.Claims
                .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? string.Empty;
         */
        var userId = user.FindFirstValue("UsuarioId");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token no válido");
        return userId;
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