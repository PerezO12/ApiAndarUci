using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
using FluentValidation;

namespace ApiUCI.Validators.Formulario
{
    public class FormularioFirmarDtoValidator : AbstractValidator<FormularioFirmarDto>
    {
        public FormularioFirmarDtoValidator()
        {
            RuleFor(x => x.LlavePrivada)
                .NotEmpty().WithMessage("La firma es requerida")
                .MaximumLength(500).WithMessage("La firma no puede tener más de 500 caracteres")
                .MinimumLength(5).WithMessage("La firma no puede tener menos de 5 caracteres");
        }
    }
}