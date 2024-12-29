using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ApiUCI.Dtos.Facultad;

namespace ApiUCI.Validators.Facultad
{
    public class FacultadUpdateDtoValidator: AbstractValidator<FacultadUpdateDto>
    {
        public FacultadUpdateDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")   
                .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres.");
        }
    }
}