
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
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly ILogger<EstudianteService> _logger;

        public EstudianteService(
            IEstudianteRepository estudianteRepository,
            UserManager<AppUser> userManager,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            ILogger<EstudianteService> logger
        )
        {
            _estudianteRepo = estudianteRepository;
            _carreraRepo = carreraRepo;
            _facuRepo = facuRepo;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<RespuestasGenerales<EstudianteDto?>> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetByIdAsync(id);
                if (estudiante == null)
                    return RespuestasGenerales<EstudianteDto?>.ErrorResponseService("Estudiante", "El estudiante no existe.");

                return RespuestasGenerales<EstudianteDto?>.SuccessResponse(estudiante.toEstudianteDto(), "Estudiante obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante {id} con detalles. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<Estudiante?>> GetEstudianteByUserId(string userId)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
                if (estudiante == null)
                    return RespuestasGenerales<Estudiante?>.ErrorResponseService("Usuario", "El usuario no existe.");

                return RespuestasGenerales<Estudiante?>.SuccessResponse(estudiante, "Estudiante obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante por UsuarioId {userId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<EstudianteDto?>> GetEstudianteWithByUserId(string userId)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
                if (estudiante == null)
                    return RespuestasGenerales<EstudianteDto?>.ErrorResponseService("Usuario", "El usuario no existe.");
                return RespuestasGenerales<EstudianteDto?>.SuccessResponse(estudiante.toEstudianteDto(), "Estudiante obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudiante por UsuarioId {userId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<List<EstudianteDto>>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query)
        {
            try
            {
                var estudiantes = await _estudianteRepo.GetAllAsync(query);
                var estudiantesDto = estudiantes.Select(e => e.toEstudianteDto()).ToList();

                return RespuestasGenerales<List<EstudianteDto>>.SuccessResponse(estudiantesDto, "Lista de estudiantes obtenida exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de estudiantes con detalles. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<NewEstudianteDto>> RegisterEstudianteAsync(RegisterEstudianteDto registerDto)
        {
            try
            {
                if (!await _facuRepo.FacultyExists(registerDto.FacultadId))
                    return RespuestasGenerales<NewEstudianteDto>.ErrorResponseService("Facultad", "La facultad no existe");
                if (!await _carreraRepo.ExisteCarrera(registerDto.CarreraId))
                    return RespuestasGenerales<NewEstudianteDto>.ErrorResponseService("Carrera", "La carrera no existe.");

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
                    return RespuestasGenerales<NewEstudianteDto>.ErrorResponseController(errores);
                }

                var roleResult = await _userManager.AddToRoleAsync(appUser, "Estudiante");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(appUser);
                    var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                    return RespuestasGenerales<NewEstudianteDto>.ErrorResponseController(errores);
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

                return RespuestasGenerales<NewEstudianteDto>.SuccessResponse(newEstudianteDto, "Estudiante creado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar un nuevo estudiante. Exception: {ex.Message}");
                throw;
            }
        }
    }

}