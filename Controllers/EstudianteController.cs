using ApiUCI.Contracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUCI.Helpers;
using ApiUCI.Interfaces;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Utilities;

namespace ApiUCI.Controller
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


        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEstudiante query)
        {
            var resultado = await _estudianteService.GetEstudiantesWithDetailsAsync(query);
            
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {

            var resultado = await _estudianteService.GetByIdWithDetailsAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet(ApiRoutes.Estudiante.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id)
        {   
            var resultado = await _estudianteService.GetEstudianteWithByUserId(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Estudiante.RegistrarEstudiante)]
        public async Task<IActionResult> RegisterEstudiante([FromBody] RegisterEstudianteDto registerDto)
        {
            var resultado = await _estudianteService.RegisterEstudianteAsync(registerDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
    }
}
