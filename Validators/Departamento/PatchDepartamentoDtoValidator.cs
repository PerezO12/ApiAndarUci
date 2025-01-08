using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ApiUci.Dtos.Departamento;

namespace ApiUci.Validators.Departamento
{
    public class PatchDepartamentoDtoValidator : AbstractValidator<PatchDepartamentoDto>
    {
        public PatchDepartamentoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .MinimumLength(3).WithMessage("El nombre del departamento es muy corto.");
            
            RuleFor(x => x.FacultadId)
                .GreaterThan(0).WithMessage("Facultad no válida.");
        }
    }
}