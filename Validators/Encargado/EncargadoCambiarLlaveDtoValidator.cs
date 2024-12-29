using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using FluentValidation;

namespace ApiUCI.Validators.Encargado
{
    public class EncargadoCambiarLlaveDtoValidator : AbstractValidator<EncargadoCambiarLlaveDto>
    {
        public EncargadoCambiarLlaveDtoValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");

            RuleFor(x => x.LlavePublica)
                .NotEmpty().WithMessage("La llave es obligatoria.");
        }
    }
}