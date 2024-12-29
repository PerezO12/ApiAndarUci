using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Usuarios;
using FluentValidation;

namespace ApiUCI.Validators.Usuario
{
    public class UsuarioWhiteRolUpdateDtoValidator : AbstractValidator<UsuarioWhiteRolUpdateDto>
    {
        public UsuarioWhiteRolUpdateDtoValidator()
        {
            RuleFor(x => x.NombreCompleto)
                .MinimumLength(10).WithMessage("El nombre completo debe tener al menos 10 caracteres.")
                .MaximumLength(100).WithMessage("El nombre completo no puede exceder los 100 caracteres.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("El nombre completo solo debe contener letras y espacios.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El formato del email es inválido.");

            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.");
            
            RuleFor(x => x.CarnetIdentidad)
                .Length(11).WithMessage("El carné de identidad debe tener exactamente 11 caracteres.")
                .Matches(@"^[0-9]*$").WithMessage("El carné de identidad solo debe contener números.");

            RuleFor(x => x.PasswordAdmin)
                .NotEmpty().WithMessage("La contraseña administrativa es requerida.");
        }
    }
}