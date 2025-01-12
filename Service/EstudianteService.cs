using ApiUci.Dtos;
using ApiUci.Extensions;
using Microsoft.AspNetCore.Identity;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Estudiante;
using ApiUci.Helpers;
using ApiUci.Interfaces;
using ApiUci.Mappers;
using ApiUci.Models;
using ApiUci.Helpers.Querys;

namespace ApiUci.Service
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFacultadRepository _facuRepo;
        private readonly IFormularioRepository _formularioRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IDepartamentoRepository _depaRepo;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<EstudianteService> _logger;

        public EstudianteService(
            IEstudianteRepository estudianteRepository,
            UserManager<AppUser> userManager,
            IFacultadRepository facuRepo,
            IDepartamentoRepository depaRepo,
            ICarreraRepository carreraRepo,
            IFormularioRepository formularioRepo,
            IUsuarioService usuarioService,
            ILogger<EstudianteService> logger
        )
        {
            _estudianteRepo = estudianteRepository;
            _carreraRepo = carreraRepo;
            _facuRepo = facuRepo;
            _userManager = userManager;
            _depaRepo = depaRepo;
            _usuarioService = usuarioService;
            _formularioRepo = formularioRepo;
            _logger = logger;
        }

        public async Task<RespuestasGenerales<EstudianteDto?>> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetByIdAsync(id);
                if (estudiante == null)
                    return RespuestasGenerales<EstudianteDto?>.ErrorResponseService("Estudiante", "El estudiante no existe.");
                var roles = await _userManager.GetRolesAsync(estudiante.AppUser!);
                return RespuestasGenerales<EstudianteDto?>.SuccessResponse(estudiante.toEstudianteDto(roles!), "Estudiante obtenido exitosamente.");
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
                if (estudiante == null || estudiante?.AppUser == null)
                    return RespuestasGenerales<EstudianteDto?>.ErrorResponseService("Usuario", "El usuario no existe.");
                var roles = await _userManager.GetRolesAsync(estudiante.AppUser);

                return RespuestasGenerales<EstudianteDto?>.SuccessResponse(estudiante.toEstudianteDto(roles), "Estudiante obtenido exitosamente.");
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
                    UserName = registerDto.UserName,
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
                    UserName = appUser.UserName!,
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

        public async Task<bool> ComprobarBajaEstudiante(int estudianteId)
        {
            var estudiante = await _estudianteRepo.GetByIdAsync(estudianteId);
            if(estudiante == null) return false;

            var departamentosCorrespondientes = await _depaRepo.GetAllDepartamentosByFacultadId(estudiante.FacultadId);
            if (departamentosCorrespondientes == null || !departamentosCorrespondientes.Any()) return false;

            var formularios = await _formularioRepo.GetAllFormulariosByEstudiante(estudiante.UsuarioId, new QueryObjectFormularioEstudiantes());
            if (formularios == null) return false;

            var departamentosIds = departamentosCorrespondientes.Select(d => d.Id).ToHashSet();

            var formulariosFirmados = formularios
                .Where(f => f.Firmado)
                .Select(f => f.DepartamentoId)
                .ToHashSet();

            var todosFirmados = departamentosIds.All(departamentoId => formulariosFirmados.Contains(departamentoId));

            if (todosFirmados)
            {
                await _usuarioService.DeleteAsync(estudiante.UsuarioId);
                await _estudianteRepo.DeleteAsync(estudianteId);
            }

            return false;
        }
    }

}