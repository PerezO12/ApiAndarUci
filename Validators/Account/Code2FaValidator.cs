using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Auth;
using FluentValidation;

namespace ApiUCI.Validators.Account
{
    public class Code2FaValidator : AbstractValidator<Code2Fa>
    {
        public Code2FaValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de autenticación es requerido.")
                .MinimumLength(6).WithMessage("El código de autenticación debe tener al menos 6 caracteres.")
                .MaximumLength(6).WithMessage("El código de autenticación no puede tener más de 6 caracteres.");
        }
    }
}