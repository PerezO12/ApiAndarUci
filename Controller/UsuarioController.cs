using System.Security.Claims;
using ApiUCI.Contracts.V1;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Extensions;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MyApiUCI.Helpers;

namespace MyApiUCI.Controller
{   
    [Authorize(Policy = "AdminPolicy")]
    [Route(ApiRoutes.Usuario.RutaGenaral)]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;
        public readonly ILogger _logger;
        public UsuarioController(IUsuarioService usuarioService, IAuthService authService, ILogger logger)
        {
            _usuarioService = usuarioService;
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
        [HttpPost(ApiRoutes.Usuario.RegistrarAdmin)]
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

                var respuesta = await _usuarioService.RegistrarAdministradorAsync(registroDto);
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


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectUsuario query)
        {
            var usuariosDto = await _usuarioService.GetAllAsync(query);
            return Ok(usuariosDto);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute]string Id)
        {
            var usuario = await _usuarioService.GetByIdAsync(Id);
            if(usuario == null) return NotFound("No existe el usuario");
            return Ok(usuario);
        }
        
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            try 
            {
                var adminId = User.GetUserId(); //todo: verificar
                if(adminId == null) return BadRequest(new {errors="Token no válido"});

                var passwordCorrecta = await _authService.VerifyUserPassword(adminId, usuarioUpdateDto.PasswordAdmin );
                if(!passwordCorrecta) return Unauthorized(new {errors="Contraseña incorrecta"});

                var resultado = await _usuarioService.UpdateAsync(id, usuarioUpdateDto);

                if(!resultado.Succeeded) return NotFound(new {msg = resultado.Errors.Select(e => e.Description).ToArray()});

                return Ok(new {msg=  "Usuario actualizado"});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new {msg = "Algo salio mal, notificar al administrador"});
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromBody]PasswordDto password) 
        {
            if(!ModelState.IsValid) return BadRequest(new {msg = "La contraseña es obligatoria"});
            try
            {
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new{msg="Token no válido"});

                var passwordCorrecta = await _authService.VerifyUserPassword(adminId, password.Password );
                if(!passwordCorrecta) return Unauthorized(new {errors="Contraseña incorrecta"});
                
                var resultado = await _usuarioService.DeleteUserYRolAsync(id, adminId);
                if(resultado.Error)
                {
                    return resultado.TipoError switch
                    {
                        "NotFound" => NotFound(new { msg = resultado.msg}),
                        "BadRequest" => BadRequest(new { msg = resultado.msg}),
                        "Unauthorized" => Unauthorized(new { msg = resultado.msg}),
                        _ => StatusCode(500, new { admin = "Falta validar esta acción. Reportar el error", msg = resultado.msg })
                    };
                }
                return Ok(new {msg = resultado.msg});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new {msg = "Ocurrió un error, informar al administrador"});
            }
        }                 
    }
}