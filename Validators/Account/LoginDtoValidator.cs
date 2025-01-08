using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ApiUci.Dtos.Cuentas;

namespace ApiUci.Validators.Account
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MinimumLength(4).WithMessage("No es un nombre válido.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede tener más de 50 caracteres.");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.");
        }
    }
}