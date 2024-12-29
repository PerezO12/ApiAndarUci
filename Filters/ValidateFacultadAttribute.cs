using ApiUCI.Interfaces.InterfacesFiltrosValidate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApiUCI.Interfaces;

namespace ApiUCI.Filters
{
    public class ValidateFacultadAttribute : Attribute, IAsyncActionFilter
    {
      private readonly IFacultadRepository _facultadRepo;

        public ValidateFacultadAttribute(IFacultadRepository facultadRepo)
        {
            _facultadRepo = facultadRepo;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {   //el nombre del objeto
            if (context.ActionArguments.TryGetValue("facultadDto", out var value) && value is IFacultadValidable facultadModel)
            {
                var exists = await _facultadRepo.FacultyExists(facultadModel.FacultadId);
                if (!exists)
                {
                    //todo: Cambiar la respuesta
                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = "La facultad especificada no existe.",
                        FacultadId = facultadModel.FacultadId
                    });
                    return;
                }
            }

            await next();
        }  
    }
}