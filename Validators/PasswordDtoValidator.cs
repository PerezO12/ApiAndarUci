using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Cuentas;
using FluentValidation;

namespace ApiUci.Validators
{
    public class PasswordDtoValidator : AbstractValidator<PasswordDto>
    {
        public PasswordDtoValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("Contraseña incorrecta.")
                .MaximumLength(255).WithMessage("La contraseña no puede tener más de 255 caracteres.");
        }
    }
}