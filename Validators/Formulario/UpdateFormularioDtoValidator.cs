using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
using FluentValidation;

namespace ApiUCI.Validators.Formulario
{
    public class UpdateFormularioDtoValidator : AbstractValidator<UpdateFormularioDto>
    {
        public UpdateFormularioDtoValidator()
        {
            RuleFor(x => x.Motivo)
                .NotEmpty().WithMessage("El motivo es requerido")
                .MaximumLength(500).WithMessage("El motivo no puede tener más de 500 caracteres")
                .MinimumLength(5).WithMessage("El motivo no puede tener menos de 5 caracteres");
        }
        
    }
}