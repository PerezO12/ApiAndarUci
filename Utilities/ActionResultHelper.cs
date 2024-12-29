using Microsoft.AspNetCore.Mvc;

namespace ApiUCI.Utilities
{
    public static class ActionResultHelper
    {
        public static IActionResult HandleActionResult(string actionResultCode, Dictionary<string, string[]>? respuesta)
        {
            return actionResultCode switch
            {
                "NotFound" => new NotFoundObjectResult(respuesta),
                "BadRequest" => new BadRequestObjectResult(respuesta),
                "Unauthorized" => new UnauthorizedObjectResult(respuesta),
                _ => new ObjectResult(new 
                { 
                    admin = "Falta validar esta acción. Reportar el error", 
                    msg = respuesta 
                })
                {
                    StatusCode = 500
                }
            };
        }
    }
}