using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Interfaces.InterfacesFiltrosValidate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApiUCI.Interfaces;

namespace ApiUCI.Filters
{
    public class ValidateCarreraAttribute
    {
        private readonly ICarreraRepository _carreraRepo;

        public ValidateCarreraAttribute(ICarreraRepository carreraRepo)
        {
            _carreraRepo = carreraRepo;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("model", out var value) && value is ICarreraValidable carreraModel)
            {
                var exists = await _carreraRepo.ExisteCarrera(carreraModel.CarreraId);
                if (!exists)
                {
                    //todo: Cambiar la respuesta
                    context.Result = new BadRequestObjectResult(new
                    {
                        Message =  " La carrera especificada no existe.",
                        CarreraId = carreraModel.CarreraId
                    });
                    return;
                }
            }

            await next();
        }  
    }
}