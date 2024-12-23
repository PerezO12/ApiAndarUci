
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly ILogger _logger;

        public EstudianteService(
            IEstudianteRepository estudianteRepository,
            UserManager<AppUser> userManager,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            ILogger logger
        )
        {
            _estudianteRepo = estudianteRepository;
            _carreraRepo = carreraRepo;
            _facuRepo = facuRepo;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<RespuestasServicios<EstudianteDto?>> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetByIdAsync(id);
                if (estudiante == null)
                {
                    var error = ErrorBuilder.Build("Estudiante", "El estudiante no existe");
                    return RespuestasServicios<EstudianteDto?>.ErrorResponse(error);
                }
                var estudianteDto = new EstudianteDto
                {
                    Id = estudiante.Id,
                    UsuarioId = estudiante.UsuarioId,
                    NombreCompleto = estudiante.AppUser!.NombreCompleto,
                    CarnetIdentidad = estudiante.AppUser.CarnetIdentidad,
                    NombreUsuario = estudiante.AppUser.UserName,
                    Email = estudiante.AppUser.Email,
                    NumeroTelefono = estudiante.AppUser.PhoneNumber,
                    NombreCarrera = estudiante.Carrera!.Nombre,
                    NombreFacultad = estudiante.Facultad!.Nombre
                };
                return RespuestasServicios<EstudianteDto?>.SuccessResponse(estudianteDto, "Estudiante obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante {id} con detalles. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasServicios<Estudiante?>> GetEstudianteByUserId(string userId)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
                if (estudiante == null)
                {
                    var error = ErrorBuilder.Build("UsuarioId", "No se encontró un estudiante asociado a este usuario");
                    return RespuestasServicios<Estudiante?>.ErrorResponse(error);
                }
                return RespuestasServicios<Estudiante?>.SuccessResponse(estudiante, "Estudiante obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante por UsuarioId {userId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasServicios<EstudianteDto?>> GetEstudianteWithByUserId(string userId)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
                if (estudiante == null)
                {
                    var error = ErrorBuilder.Build("UsuarioId", "No se encontró un estudiante asociado a este usuario");
                    return RespuestasServicios<EstudianteDto?>.ErrorResponse(error);
                }

                var estudianteDto = new EstudianteDto
                {
                    Id = estudiante.Id,
                    UsuarioId = estudiante.UsuarioId,
                    NombreCompleto = estudiante.AppUser!.NombreCompleto,
                    CarnetIdentidad = estudiante.AppUser.CarnetIdentidad,
                    NombreUsuario = estudiante.AppUser.UserName,
                    Email = estudiante.AppUser.Email,
                    NumeroTelefono = estudiante.AppUser.PhoneNumber,
                    NombreCarrera = estudiante.Carrera!.Nombre,
                    NombreFacultad = estudiante.Facultad!.Nombre
                };

                return RespuestasServicios<EstudianteDto?>.SuccessResponse(estudianteDto, "Estudiante obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante por UsuarioId {userId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasServicios<List<EstudianteDto>>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query)
        {
            try
            {
                var estudiantes = await _estudianteRepo.GetAllAsync(query);
                var estudiantesDto = estudiantes.Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    UsuarioId = e.UsuarioId,
                    NombreCompleto = e.AppUser!.NombreCompleto,
                    CarnetIdentidad = e.AppUser.CarnetIdentidad,
                    NombreUsuario = e.AppUser.UserName,
                    Email = e.AppUser.Email,
                    NumeroTelefono = e.AppUser.PhoneNumber,
                    NombreCarrera = e.Carrera!.Nombre,
                    NombreFacultad = e.Facultad!.Nombre
                }).ToList();

                return RespuestasServicios<List<EstudianteDto>>.SuccessResponse(estudiantesDto, "Lista de estudiantes obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de estudiantes con detalles. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasServicios<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto)
        {
            try
            {
                if (!await _facuRepo.FacultyExists(registerDto.FacultadId))
                {
                    var error = ErrorBuilder.Build("FacultadId", "La facultad no existe");
                    return RespuestasServicios<NewEstudianteDto>.ErrorResponse(error);
                }

                if (!await _carreraRepo.ExisteCarrera(registerDto.CarreraId))
                {
                    var error = ErrorBuilder.Build("CarreraId", "La carrera no existe");
                    return RespuestasServicios<NewEstudianteDto>.ErrorResponse(error);
                }

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
                    await _userManager.DeleteAsync(appUser);
                    var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                    return RespuestasServicios<NewEstudianteDto>.ErrorResponse(errores);
                }

                var estudiante = new Estudiante
                {
                    UsuarioId = appUser.Id,
                    CarreraId = registerDto.CarreraId,
                    FacultadId = registerDto.FacultadId
                };

                await _estudianteRepo.CreateAsync(estudiante);

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
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar un nuevo estudiante. Exception: {ex.Message}");
                throw;
            }
        }
    }

}