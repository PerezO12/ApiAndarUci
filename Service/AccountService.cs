using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using ApiUCI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly IEncargadoService _encargadoService;
        private readonly IDepartamentoRepository _depaRepo;
        private readonly ApplicationDbContext _context;

        public AccountService(          
            UserManager<AppUser> userManager,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService,
            IDepartamentoRepository depaRepo,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _facuRepo = facuRepo;
            _carreraRepo = carreraRepo;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService;
            _depaRepo = depaRepo;
            _context = context;
        }

        public async Task<RespuestasServicios<NewEncargadoDto>> RegisterEncargadoAsync(RegisterEncargadoDto registerDto)
        {
            // Verificar si el departamento existe
            if (!await _depaRepo.ExistDepartamento(registerDto.DepartamentoId))
            {
                var errors = ErrorBuilder.Build("DepartamentoId", "El departamento no existe");
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errors);
            }

            // Verificar si ya existe un encargado en el departamento
            if (await _depaRepo.TieneEncargado(registerDto.DepartamentoId))
            {
                var errors = ErrorBuilder.Build("DepartamentoId", "Ya existe un encargado en el departamento");
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errors);
            }

            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.NombreUsuario,
                Email = registerDto.Email,
                NombreCompleto = registerDto.NombreCompleto,
                CarnetIdentidad = registerDto.CarnetIdentidad
            };

            // Crear el usuario en la base de datos
            var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errores = ErrorBuilder.ParseIdentityErrors(createUserResult.Errors);
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errores);
            }

            // Asignar el rol de encargado
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Encargado");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir la creación del usuario si falla la asignación de rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errores);
            }

            try
            {
                // Actualizar el departamento
                await _encargadoService.CreateAsync(new Encargado
                {
                    UsuarioId = appUser.Id,
                    DepartamentoId = registerDto.DepartamentoId
                });
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser); // Revertir la creación del usuario si falla la creación del encargado
                var errors = ErrorBuilder.Build("Exception", $"Error al guardar el encargado: {ex.Message}");
                throw;
            }

            // Crear el DTO de encargado
            var newEncargadoDto = new NewEncargadoDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Encargado" }
            };

            return RespuestasServicios<NewEncargadoDto>.SuccessResponse(newEncargadoDto, "Encargado creado exitosamente");
        }

        public async Task<RespuestasServicios<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto)
        {
            // Verificar si la facultad existe
            if (!await _facuRepo.FacultyExists(registerDto.FacultadId)) 
            {
                var errors = ErrorBuilder.Build("FacultadId", "La facultad no existe");
                return RespuestasServicios<NewEstudianteDto>.ErrorResponse(errors);
            }

            // Verificar si la carrera existe
            if (!await _carreraRepo.ExisteCarrera(registerDto.CarreraId)) 
            {
                var errors = ErrorBuilder.Build("CarreraId", "La carrera no existe");
                return RespuestasServicios<NewEstudianteDto>.ErrorResponse(errors);
            }

            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.nombreUsuario,
                Email = registerDto.Email,
                NombreCompleto = registerDto.NombreCompleto,
                CarnetIdentidad = registerDto.CarnetIdentidad
            };

            var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createUserResult.Succeeded) 
            {
                var errores = ErrorBuilder.ParseIdentityErrors(createUserResult.Errors);
                return RespuestasServicios<NewEstudianteDto>.ErrorResponse(errores);
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "Estudiante");
            if (!roleResult.Succeeded) 
            {
                await _userManager.DeleteAsync(appUser); // Revertir creación del usuario
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasServicios<NewEstudianteDto>.ErrorResponse(errores);
            }

            var estudiante = new Estudiante
            {
                UsuarioId = appUser.Id,
                CarreraId = registerDto.CarreraId,
                FacultadId = registerDto.FacultadId
            };

            try
            {
                await _estudianteRepo.CreateAsync(estudiante);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser);
                var errors = ErrorBuilder.Build("Exception", $"Error al crear el estudiante: {ex.Message}");
                throw;
            }

            var newEstudianteDto = new NewEstudianteDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Estudiante" }
            };

            return RespuestasServicios<NewEstudianteDto>.SuccessResponse(newEstudianteDto, "Estudiante creado exitosamente");
        }

        public async Task<RespuestasServicios<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto)
        {
            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registroDto.NombreUsuario,
                Email = registroDto.Email,
                NombreCompleto = registroDto.NombreCompleto,
                CarnetIdentidad = registroDto.CarnetIdentidad
            };

            // Crear el usuario en la base de datos
            var createUserResult = await _userManager.CreateAsync(appUser, registroDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errores = ErrorBuilder.ParseIdentityErrors(createUserResult.Errors);
                return RespuestasServicios<NewAdminDto>.ErrorResponse(errores);
            }

            // Asignar rol de administrador
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir creación del usuario si falla la asignación del rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasServicios<NewAdminDto>.ErrorResponse(errores);
            }

            // Crear el DTO para el administrador
            var newAdminDto = new NewAdminDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Admin" }
            };

            // Devolver éxito con el DTO
            return RespuestasServicios<NewAdminDto>.SuccessResponse(newAdminDto, "Administrador creado exitosamente");
        }
    }
}