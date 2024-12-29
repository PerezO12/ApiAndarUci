using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Interfaces;
using ApiUCI.Interfaces.InterfacesFiltrosValidate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApiUCI.Interfaces;

namespace ApiUCI.Filters
{
    public class ValidateDepartamentoAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IDepartamentoRepository _depaRepo;

        public ValidateDepartamentoAttribute(IDepartamentoRepository depaService)
        {
            _depaRepo = depaService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("model", out var value) && value is IDepartamentoValidable departamentoModel)
            {
                var exists = await _depaRepo.ExistDepartamento(departamentoModel.DepartamentoId);
                if (!exists)
                {
                    //todo: Cambiar la respuesta
                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = "El departamento especificada no existe.",
                        DepartamentoId = departamentoModel.DepartamentoId
                    });
                    return;
                }
            }

            await next();
        }  
    }
}