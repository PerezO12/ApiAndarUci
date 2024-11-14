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

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormularioController : ControllerBase
    {
        private readonly IFormularioService _formularioService;
        private readonly IFormularioRepository _formularioRepo;
        private readonly IDepartamentoRepository _departamentoRepo;
        private readonly ILogger<FormularioController> _logger;

        public FormularioController(
            IFormularioService formularioService, 
            IFormularioRepository formularioRepo,
            IDepartamentoRepository departamentoRepo,
            ILogger<FormularioController> logger)
        {
            _formularioRepo = formularioRepo;
            _formularioService = formularioService;
            _departamentoRepo = departamentoRepo;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormularioDto formularioDto)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Modelo inválido al intentar crear un formulario.");
                return BadRequest(ModelState);
            }

            // Obtener el Id del usuario desde el token
            var UsuarioIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            string? usuarioId = UsuarioIdClaim?.Value;
            if (usuarioId == null)
            {
                _logger.LogWarning("El token no contiene el Id del usuario.");
                return BadRequest("Token no valido.");
            }

            // Verificar existencia del departamento
            var departamento = await _departamentoRepo.GetByIdAsync(formularioDto.DepartamentoId);
            if (departamento == null)
            {
                _logger.LogWarning("El departamento con Id {DepartamentoId} no existe.", formularioDto.DepartamentoId);
                return BadRequest("El departamento no existe");
            }

            try
            {
                _logger.LogInformation("Iniciando la creación del formulario.");
                var formularioCreado = await _formularioRepo.CreateAsync(formularioDto.toFormularioFromCreate(usuarioId, departamento.FacultadId));
                
                if (formularioCreado == null)
                {
                    _logger.LogWarning("El usuario al que pertenece este formulario no existe.");
                    return NotFound("El usuario al que pertenece este formulario no existe.");
                }

                _logger.LogInformation("Formulario creado exitosamente con Id {Id}.", formularioCreado.Id);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = formularioCreado.Id },
                    new { message = "Formulario creado exitosamente", id = formularioCreado.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el formulario: {ex.Message}", ex);
                return StatusCode(500, $"Error interno al crear el formulario: {ex.Message}");
            }
        }
    }
}
