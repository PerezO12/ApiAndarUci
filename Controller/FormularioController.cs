using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using Microsoft.Extensions.Logging;
using MyApiUCI.Mappers;
using ApiUCI.Dtos.Formulario;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApiUCI.Dtos;
using ApiUCI.Helpers.Querys;
using ApiUCI.Contracts.V1;

namespace MyApiUCI.Controller
{
    [Route(ApiRoutes.Formulario.RutaGenaral)]
    [ApiController]
    public class FormularioController : ControllerBase
    {
        private readonly IFormularioService _formularioService;
        private readonly ILogger<FormularioController> _logger;
        public FormularioController(
            IFormularioService formularioService,
            ILogger<FormularioController> logger)
        {
            _formularioService = formularioService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectFormulario query)
        {
            try
            {
                _logger.LogInformation("Iniciando la obtención de formularios.");
                var formularios = await _formularioService.GetAllFormulariosWhithDetailsAsync(query);
                if (formularios == null || !formularios.Any())
                {
                    _logger.LogWarning("No se encontraron formularios con los parámetros proporcionados.");
                    return NotFound(new { message = "No se encontraron formularios." });
                }
                _logger.LogInformation("Formularios obtenidos con éxito.");
                return Ok(formularios);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener formularios: {ex.Message}", ex);
                return StatusCode(500, $"Error interno al obtener formularios: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Iniciando la obtención del formulario con Id {Id}.", id);
                var formulario = await _formularioService.GetFormularioWithDetailsAsync(id);
                if (formulario == null)
                {
                    _logger.LogWarning("Formulario con Id {Id} no encontrado.", id);
                    return NotFound(new { message = "Formulario no encontrado" });
                }
                _logger.LogInformation("Formulario con Id {Id} obtenido con éxito.", id);
                return Ok(formulario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el formulario con Id {id}: {ex.Message}", ex);
                return StatusCode(500, $"Error interno al obtener el formulario con Id {id}: {ex.Message}");
            }
        }
        //Este controlador es para q el estudiante que este autenticado obtenga sus formularios
        //TODO: poner para que el estudiante sea el q pueda acceder a esto
        
        [Authorize(Policy = "EstudiantePolicy")]
        [HttpGet(ApiRoutes.Formulario.GetFormularioEstudiante)]
        public async Task<IActionResult> GetFormulariosEstudiante([FromQuery] QueryObjectFormularioEstudiantes query){
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            if(usuarioId == null) 
            {
                _logger.LogInformation("El token no contiene el id del usuario");
                return BadRequest("Token no válido");
            }
            _logger.LogInformation("Usuario autenticado con Id: {UsuarioId}", usuarioId);

            var formularios = await _formularioService.GetAllFormulariosEstudiantesAsync(usuarioId, query);

            if(formularios == null) {
                _logger.LogInformation("El usuario no tiene formularios");
                return NotFound("No hay formularios");
            }

            return Ok(formularios);

        }

        [Authorize(Policy = "EncargadoPolicy")]
        [HttpGet(ApiRoutes.Formulario.GetAllFormulariosByEncargado)]
        public async Task<IActionResult> GetAllFormulariosByEncargado([FromQuery] QueryObjectFormularioEncargado query)
        {
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            if( usuarioId == null )
            {
                _logger.LogInformation("El token no contiene el id del usuario");
                return BadRequest("Token no válido");
            }
            _logger.LogInformation("Usuario autenticado con Id: {UsuarioId}", usuarioId);

            var formularios = await _formularioService.GetAllFormulariosEncargadosAsync(usuarioId, query);

            if(formularios == null) {
                _logger.LogInformation("El usuario no tiene formularios");
                return NotFound("No hay formularios");
            }

            return Ok(formularios);
        }

        [Authorize(Policy = "EncargadoPolicy")]
        [HttpGet(ApiRoutes.Formulario.GetFormularioEstudianteWhitDetails)]
        public async Task<IActionResult> GetFormularioEstudianteWhitDetails([FromRoute] int id)
        {
            var userId = User.FindFirst("UsuarioId")?.Value;
            if(userId == null) return BadRequest("Token no válido");

            var formulario = await _formularioService.GetFormEstudianteByIdForEncargadoAsync(userId, id);
            if(formulario == null) return BadRequest("No existe el formulario");
            
            return Ok(formulario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormularioDto formularioDto)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Modelo inválido al intentar crear un formulario.");
                return BadRequest(ModelState);
            }

            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            //var usuarioId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (usuarioId == null)
            {
                _logger.LogWarning("El token no contiene el Id del usuario.");
                return BadRequest("Token no válido.");
            }
            _logger.LogInformation("Usuario autenticado con Id: {UsuarioId}", usuarioId);

            // Verificar existencia del departamento        

            try
            {
                _logger.LogInformation("Iniciando la creación del formulario.");
                var resultado = await _formularioService.CreateFormularioAync(usuarioId, formularioDto);
                
                if (resultado.Error && resultado.TipoError.Contains("BadRequest"))
                {
                    return BadRequest(new {msg = resultado.msg});
                }

                return StatusCode(201, new {msg = resultado.msg});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el formulario: {ex.Message}", ex);
                return StatusCode(500, $"Error interno al crear el formulario: {ex.Message}");
            }
        }
    
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody]UpdateFormularioDto formularioDto)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Modelo inválido al intentar crear un formulario.");
                return BadRequest(ModelState);
            }

            var usuarioId = User.FindFirst("UsuarioId")?.Value;

            if (usuarioId == null)
            {
                _logger.LogWarning("El token no contiene el Id del usuario.");
                return BadRequest("Token no válido.");
            }
            _logger.LogInformation("Usuario autenticado con Id: {UsuarioId}", usuarioId);

            var formulario = await _formularioService.UpdatePatchFormularioAsync(usuarioId, id, formularioDto);
        
            if(formulario.Error) {
                return formulario.TipoError switch
                {
                    "NotFound" => NotFound(new { msg = formulario.msg }),
                    "BadRequest" => BadRequest(new { msg = formulario.msg }),
                    "Unauthorized" => Unauthorized(new { msg = formulario.msg }),
                    _ => StatusCode(500, new { admin = "Falta validar esta acción. Reportar el error", msg = formulario.msg })
                };
            }     

            return Ok(new{ msg = "Formulario actualizado correctamente"});
        }

        //solo encargado
        [HttpPatch(ApiRoutes.Formulario.FirmarFormulario)]
        public async Task<IActionResult> FirmarFormulario([FromRoute] int id, [FromBody] FormularioFirmarDto formularioDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            if(usuarioId == null) return BadRequest("Token no válido");
            try
            {
                var respuesta = await _formularioService.FirmarFormularioAsync(usuarioId, id, formularioDto);
                //Validaciones
            if (respuesta.Error)
                {
                    return respuesta.TipoError switch
                    {
                        var tipo when tipo.Contains("NotFound") => NotFound(new { msg = respuesta.msg }),
                        var tipo when tipo.Contains("Unauthorized") => Unauthorized(new { msg = respuesta.msg }),
                        var tipo when tipo.Contains("BadRequest") => BadRequest(new { msg = respuesta.msg }),
                        var tipo when tipo.Contains("StatusCode500") => StatusCode(500, new { msg = respuesta.msg }),
                        _ => StatusCode(500, new { admin = "Falta validar esta acción, Reportar el error", msg = respuesta.msg })
                    };
                    }
            return Ok( new { msg = respuesta.msg});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el formulario: {ex.Message}", ex);
                return StatusCode(500, $"Error interno al actualizar el formulario: {ex.Message}");
            }
        }
    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormularioEstudiante([FromRoute]int id)
        {
            var userId = User.FindFirst("UsuarioId")?.Value;
            if(userId == null ) return BadRequest("Token no válido");

            var resultado = await _formularioService.DeleteFormularioEstudianteAsync(userId, id);
        
            if(resultado.Error)
            {
                return resultado.TipoError switch
                {
                    "NotFound" => NotFound(new { msg = resultado.msg }),
                    "BadRequest" => BadRequest(new { msg = resultado.msg }),
                    "Unauthorized" => Unauthorized(new { msg = resultado.msg }),
                    _ => StatusCode(500, new { admin = "Falta validar esta acción. Reportar el error", msg = resultado.msg })
                };
            }

            return Ok(new { msg = resultado.msg });
        }
/*         //agregar prote
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> DeleteFormularioAdmin([FromRoute] int id)
        {
            var resultado = await _formularioService.DeleteFormularioAdmin(id);
            if(resultado.Error){
                return BadRequest(new{ msg= resultado.msg} );
            }
            return Ok(new { msg = resultado.msg });
        } */


    }
}
