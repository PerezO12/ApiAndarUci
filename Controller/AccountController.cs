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
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Centraliza el manejo de respuestas exitosas y errores
        private IActionResult HandleResponse<T>(RespuestasServicios<T> respuesta)
        {
            return respuesta.Success ? Ok(respuesta) : BadRequest(respuesta);
        }

        private IActionResult HandleError(Exception ex, string customMessage = "Error en el servidor", string property = "General")
        {
            _logger.LogError(ex, customMessage);
            var error = ErrorBuilder.Build(property, customMessage);
            return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
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
                return HandleError(ex, "Error en el login", "Login");
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
                return HandleError(ex, "Error al obtener perfil para el usuario", "Perfil");
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
                return HandleError(ex, "Error al cambiar contraseña para el usuario", "CambioContraseña");
            }
        }
    }
}