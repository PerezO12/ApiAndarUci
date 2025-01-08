using FluentValidation;
using ApiUci.Dtos.Cuentas;

namespace ApiUci.Validators.Estudiante
{
    public class RegisterEstudianteDtoValidator : AbstractValidator<RegisterEstudianteDto>
    {
        public RegisterEstudianteDtoValidator()
        {
            // Validación para userName
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .Matches(@"^\S*$").WithMessage("El nombre de usuario no debe contener espacios.");

            // Validación para Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("No es un correo válido.");

            // Validación para Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.");

            // Validación para NombreCompleto
            RuleFor(x => x.NombreCompleto)
                .NotEmpty().WithMessage("El nombre completo es requerido.")
                .MinimumLength(10).WithMessage("El nombre completo debe tener al menos 10 caracteres.")
                .MaximumLength(100).WithMessage("El nombre completo no puede exceder los 100 caracteres.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("El nombre completo solo debe contener letras y espacios.");

            // Validación para CarnetIdentidad
            RuleFor(x => x.CarnetIdentidad)
                .NotEmpty().WithMessage("El carné de identidad es requerido.")
                .Length(11).WithMessage("El carné de identidad debe tener exactamente 11 números.")
                .Matches(@"^\d{11}$").WithMessage("El carné de identidad debe contener solo números.");

            // Validación para CarreraId
            RuleFor(x => x.CarreraId)
                .GreaterThan(0).WithMessage("Carrera no válida.");

            // Validación para FacultadId
            RuleFor(x => x.FacultadId)
                .GreaterThan(0).WithMessage("Facultad no válida.");
        }
    }
}
