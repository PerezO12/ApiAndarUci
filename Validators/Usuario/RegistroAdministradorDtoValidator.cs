using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Dtos.Cuentas;
using FluentValidation;

namespace ApiUci.Validators.Usuario
{
    public class RegistroAdministradorDtoValidator : AbstractValidator<RegistroAdministradorDto>
    {
        public RegistroAdministradorDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .Matches(@"^\S*$").WithMessage("El nombre de usuario no debe contener espacios.");

            RuleFor(x => x.NombreCompleto)
                .NotEmpty().WithMessage("El nombre completo es requerido.")
                .MinimumLength(10).WithMessage("El nombre completo debe tener al menos 10 caracteres.")
                .MaximumLength(100).WithMessage("El nombre completo no puede exceder los 100 caracteres.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("El nombre completo solo debe contener letras y espacios.");

            RuleFor(x => x.Email)
                .NotEmpty().When(x => x.Email != null).WithMessage("El email no puede estar vacío si se proporciona.")
                .EmailAddress().WithMessage("El formato del email es inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.");
            
            RuleFor(x => x.CarnetIdentidad)
                .NotEmpty().WithMessage("El carné de identidad es obligatorio.")
                .Length(11).WithMessage("El carné de identidad debe tener exactamente 11 caracteres.")
                .Matches(@"^[0-9]*$").WithMessage("El carné de identidad solo debe contener números.");

            // Validar PasswordAdmin
/*             RuleFor(x => x.PasswordAdmin)
                .NotEmpty().WithMessage("La contraseña administrativa es requerida."); */
        }
    }
}