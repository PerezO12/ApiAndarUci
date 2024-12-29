using ApiUCI.Dtos.Formulario;
using ApiUCI.Models;

    namespace ApiUCI.Mappers
    {
        public static class FormularioMappers
        {
            public static Formulario toFormularioFromCreate(this CreateFormularioDto formularioDto, Estudiante estudiante, Encargado encargado)
            {
                return new Formulario{
                    EstudianteId = estudiante.Id,
                    EncargadoId = encargado.Id,
                    DepartamentoId = formularioDto.DepartamentoId,
                    Motivo = formularioDto.Motivo
                };
            }

            public static Formulario toPatchFormulario(this Formulario formularioModel, UpdateFormularioDto formularioDto)
            {
/*                 formularioModel.FirmadoPorId = formularioDto.FirmadoPor ??  formularioModel.FirmadoPorId; */
                //formularioModel.DepartamentoId = formularioDto.DepartamentoId ?? formularioModel.DepartamentoId;
/*                 formularioModel.FacultadId = formularioDto.FacultadId ?? formularioModel.FacultadId;
                formularioModel.Firmado = formularioDto.Firmado ?? formularioModel.Firmado;
                formularioModel.FirmaEncargado = formularioDto.FirmaEncargado ?? formularioModel.FirmaEncargado;
                formularioModel.FechaFirmado = formularioDto.FechaFirmado ?? formularioModel.FechaFirmado; */
                formularioModel.Motivo = formularioDto.Motivo ?? formularioModel.Motivo;

                return formularioModel;
            }
            public static FormularioDto toFormularioDtoWithDetails(this Formulario formularioModel)
            {
                return  new FormularioDto {
                    id = formularioModel.Id,
                    NombreEstudiante = formularioModel.Estudiante!.AppUser!.NombreCompleto,
                    NombreUsuarioEstudiante = formularioModel.Estudiante.AppUser.UserName,
                    CarnetIdentidadEstudiante = formularioModel.Estudiante.AppUser.CarnetIdentidad,
                    EmailEstudiante = formularioModel.Estudiante.AppUser.Email,
                    NombreCarrera = formularioModel.Estudiante.Carrera!.Nombre,
                    NombreDepartamento = formularioModel.Departamento!.Nombre,
                    NombreFacultad = formularioModel.Departamento.Facultad!.Nombre,
                    Motivo = formularioModel.Motivo,
                    NombreEncargado = formularioModel.Encargado!.AppUser!.NombreCompleto,
                    Firmado = formularioModel.Firmado,
                    FechaFirmado = formularioModel.FechaFirmado,
                    Fechacreacion = formularioModel.Fechacreacion
                };
            }
            public static FormularioEncargadoDto toFormularioEncargadoDto(this Formulario FormularioModel)
            {
                return new FormularioEncargadoDto{
                    Id = FormularioModel.Id,
                    NombreCompletoEstudiante = FormularioModel.Estudiante!.AppUser!.NombreCompleto!,
                    Firmado = FormularioModel.Firmado,
                    NombreCarrera = FormularioModel.Estudiante!.Carrera!.Nombre,
                    Motivo = FormularioModel.Motivo,
                    Fechacreacion = FormularioModel.Fechacreacion
                };
            }
            public static FormularioEstudianteDto toFormularioEstudianteDto(this Formulario FormularioModel)
            {
                return new FormularioEstudianteDto{
                    Id = FormularioModel.Id,
                    NombreEncargado = FormularioModel.Encargado!.AppUser!.NombreCompleto!,
                    NombreDepartamento= FormularioModel.Departamento!.Nombre,
                    Motivo = FormularioModel.Motivo,
                    Firmado = FormularioModel.Firmado,
                    FechaFirmado = FormularioModel.FechaFirmado,
                    Fechacreacion = FormularioModel.Fechacreacion
                };
            }
        }
    }