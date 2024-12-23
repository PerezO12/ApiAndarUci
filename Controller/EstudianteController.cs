using System;
using System.Threading.Tasks;
using ApiUCI.Contracts.V1;
using ApiUCI.Dtos;
using ApiUCI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using Microsoft.Extensions.Logging;
using MyApiUCI.Dtos.Cuentas;

namespace MyApiUCI.Controller
{
    [ApiController]
    [Route(ApiRoutes.Estudiante.RutaGenaral)]
    public class EstudianteController : ControllerBase
    {
        private readonly IEstudianteService _estudianteService;
        private readonly ILogger<EstudianteController> _logger;

        public EstudianteController(IEstudianteService estudianteService, ILogger<EstudianteController> logger)
        {
            _estudianteService = estudianteService;
            _logger = logger;
        }

        // Centraliza el manejo de respuestas exitosas y errores
        private IActionResult HandleResponse<T>(RespuestasServicios<T> respuesta)
        {
            if (respuesta.Success)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        private IActionResult HandleError(Exception ex, string customMessage = "Error en el servidor", string property = "General")
        {
            _logger.LogError(ex, customMessage);
            var error = ErrorBuilder.Build(property, customMessage);
            return StatusCode(500, RespuestasServicios<string>.ErrorResponse(error, ex.Message));
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEstudiante query)
        {
            try
            {
                var estudiantes = await _estudianteService.GetEstudiantesWithDetailsAsync(query);
                return Ok(estudiantes);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error al obtener los estudiantes", "Estudiantes");
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var estudiante = await _estudianteService.GetByIdWithDetailsAsync(id);
                if (estudiante == null)
                    return NotFound(new { msg = "El estudiante no existe" });

                return Ok(estudiante);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error al obtener el estudiante por ID", "EstudianteId");
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet(ApiRoutes.Estudiante.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id)
        {
            try
            {
                var estudiante = await _estudianteService.GetEstudianteWithByUserId(id);
                if (estudiante == null)
                    return NotFound(new { msg = "El estudiante no existe" });

                return Ok(estudiante);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error al obtener el estudiante por ID de usuario", "UserId");
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Estudiante.RegistrarEstudiante)]
        public async Task<IActionResult> RegisterEstudiante([FromBody] RegisterEstudianteDto registerDto)
        {
            try
            {
                var respuesta = await _estudianteService.RegisterEstudianteAsync(registerDto);
                return HandleResponse(respuesta);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error al registrar estudiante", "Registro");
            }
        }
    }
}
