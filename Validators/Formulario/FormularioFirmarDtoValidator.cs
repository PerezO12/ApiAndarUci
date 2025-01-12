using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Formulario;
using FluentValidation;

namespace ApiUci.Validators.Formulario
{
    public class FormularioFirmarDtoValidator : AbstractValidator<FormularioFirmarDto>
    {
        public FormularioFirmarDtoValidator()
        {
            RuleFor(x => x.LlavePrivada)
                .NotEmpty().WithMessage("La firma es requerida")
                .MaximumLength(2000).WithMessage("La llave no es válida")
                .MinimumLength(5).WithMessage("La llave no es válida");
        }
    }
}