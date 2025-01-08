using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Encargado;
using FluentValidation;

namespace ApiUci.Validators.Encargado
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