using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ApiUCI.Dtos.Carrera;

namespace ApiUCI.Validators.Carrera
{
    public class UpdateCarreraDtoValidator : AbstractValidator<UpdateCarreraDto>
    {
        public UpdateCarreraDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la carrera es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre es muy corto.");
            
            RuleFor(x => x.FacultadId)
                .NotEmpty().WithMessage("La facultad es obligatoria.")
                .GreaterThan(0).WithMessage("Facultad no válida.");
        }
    }
}