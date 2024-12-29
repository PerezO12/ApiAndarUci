using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ApiUCI.Dtos.Departamento;

namespace ApiUCI.Validators.Departamento
{
    public class UpdateDepartamentoDtoValidator : AbstractValidator<UpdateDepartamentoDto>
    {
        public UpdateDepartamentoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del departamento es requerido.")
                .MinimumLength(3).WithMessage("El nombre del departamento es muy corto.");
            
            RuleFor(x => x.FacultadId)
                .NotEmpty().WithMessage("La facultad es requerida.")
                .GreaterThan(0).WithMessage("Facultad no válida.");
        }
    }
}