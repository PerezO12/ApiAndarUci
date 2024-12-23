using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiUCI.Contracts.V1;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using ApiUCI.Interfaces;
using ApiUCI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;

namespace MyApiUCI.Controller
{
    [Route(ApiRoutes.Account.RutaGenaral)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _acountService;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AccountController(
            IAccountService acountService, 
            IAuthService authService,
            ILogger<AccountController> logger
        )
        {
            _acountService = acountService;
            _authService = authService;
            _logger = logger;
        }

        private IActionResult HandleResponse<T>(RespuestasServicios<T> respuesta)
        {
            if (respuesta.Success)
            {
                return Ok(respuesta);
            }
            return BadRequest(respuesta);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Account.RegistrarAdmin)]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistroAdministradorDto registroDto) 
        {
            try
            {
                var adminId = User.GetUserId();
                var passwordResult = await _authService.VerifyUserPassword(adminId, registroDto.PasswordAdmin);
                if (!passwordResult)
                {
                    var error = ErrorBuilder.Build("Password", "Contraseña incorrecta");
                    return Unauthorized(RespuestasServicios<string>.ErrorResponse(error, "No autorizado"));
                }

                var respuesta = await _acountService.RegistrarAdministradorAsync(registroDto);
                return HandleResponse(respuesta);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Token no válido");
                var error = ErrorBuilder.Build("Token", "Token no válido");
                return BadRequest(RespuestasServicios<string>.ErrorResponse(error, "Acceso denegado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar administrador para el usuario {AdminId}", User.GetUserId());
                var error = ErrorBuilder.Build("General", "Contactar al administrador");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, "Error interno del servidor"));
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Account.RegistrarEstudiante)]
        public async Task<IActionResult> RegisterEstudiante([FromBody] RegisterEstudianteDto registerDto)
        {
            try
            {
                var respuesta = await _acountService.RegisterEstudianteAsync(registerDto);
                return HandleResponse(respuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar estudiante");
                var error = ErrorBuilder.Build("General", "Error en el servidor");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Account.RegistrarEncargado)]
        public async Task<IActionResult> RegisterEncargado([FromBody] RegisterEncargadoDto registerDto)
        {
            try
            {
                var respuesta = await _acountService.RegisterEncargadoAsync(registerDto);
                return HandleResponse(respuesta);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Token no válido");
                var error = ErrorBuilder.Build("Token", "Token no válido");
                return BadRequest(RespuestasServicios<string>.ErrorResponse(error, "Acceso denegado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar encargado para el usuario {AdminId}", User.GetUserId());
                var error = ErrorBuilder.Build("General", "Error en el servidor");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
            }
        }

        [HttpPost(ApiRoutes.Account.Login)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var resultado = await _authService.Login(loginDto);
                return HandleResponse(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el login");
                var error = ErrorBuilder.Build("Login", "Ocurrió un error, informar al administrador");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
            }
        }

        [Authorize]
        [HttpGet(ApiRoutes.Account.ObtenerPerfil)]
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var userId = User.GetUserId();
                var respuesta = await _authService.ObtenerPerfilAsync(userId);
                return HandleResponse(respuesta);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Token no válido");
                var error = ErrorBuilder.Build("Token", "Token no válido");
                return BadRequest(RespuestasServicios<string>.ErrorResponse(error, "Acceso denegado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil para el usuario {UserId}", User.GetUserId());
                var error = ErrorBuilder.Build("General", "Error en el servidor");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
            }
        }

        [HttpPost(ApiRoutes.Account.CambiarPassword)]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto cuentadDto)
        {
            try
            {
                var userId = User.GetUserId();
                var usuario = await _authService.ExisteUsuario(userId);
                if (usuario == null)
                {
                    var error = ErrorBuilder.Build("Usuario", "El usuario no existe");
                    return BadRequest(RespuestasServicios<string>.ErrorResponse(error, "Usuario no encontrado"));
                }

                var resultado = await _authService.CambiarPasswordAsync(usuario, cuentadDto);
                return HandleResponse(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Token no válido");
                var error = ErrorBuilder.Build("Token", "Token no válido");
                return BadRequest(RespuestasServicios<string>.ErrorResponse(error, "Acceso denegado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para el usuario {UserId}", User.GetUserId());
                var error = ErrorBuilder.Build("General", "Error en el servidor");
                return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
            }
        }
    }
}
