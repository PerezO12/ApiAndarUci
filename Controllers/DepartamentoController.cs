using ApiUCI.Contracts.V1;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using ApiUCI.Helpers.Querys;
using ApiUCI.Interfaces;
using ApiUCI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUCI.Dtos.Departamento;


namespace ApiUCI.Controller
{
    [Route(ApiRoutes.Departamento.RutaGenaral)]
    [ApiController]
    public class DepartamentoController : ControllerBase 
    {
        private readonly IDepartamentoService _depaService;
        private readonly IAuthService _authService;

        public DepartamentoController(
            IDepartamentoService depaService,
            IAuthService authService
        )
        {
            _depaService = depaService;
            _authService = authService;
        }

        //Get
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll( [FromQuery] QueryObjectDepartamentos query)
        {
            var respuesta = await _depaService.GetAllAsync(query);
            return Ok(respuesta.Data);

        }

        //GetByID
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var respuesta = await _depaService.GetByIdAsync(id);
            if(!respuesta.Success)
                return ActionResultHelper.HandleActionResult(respuesta.ActionResult, respuesta.Errors);

            return Ok(respuesta.Data);
        }

        //Post
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create( [FromBody]CreateDepartamentoDto departamentoDto)
        {
            var resultado = await _depaService.CreateAsync(departamentoDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return CreatedAtAction(nameof(GetById), new {id = resultado?.Data?.Id}, resultado?.Data);
        }

        //Put
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateDepartamentoDto departamentoDto)
        {
            var resultado = await _depaService.UpdateAsync(id, departamentoDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            return Ok(resultado.Data);
        }

        //Delete
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id, [FromBody]PasswordDto password)
        {
            //TODO: hacer un filtro
            var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), password.Password);
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);

            }
            var resultado = await _depaService.DeleteAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data); 
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchDepartamentoDto departamentoDto)
        {
            var resultado = await _depaService.PatchAsync(id, departamentoDto);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data); 

        }

        [Authorize(Policy = "EstudiantePolicy")]
        [HttpGet("correspondientes")]
        public async Task<IActionResult> GetAllDepartamentosByEstudiante()
        {
            var resultado = await _depaService.GetAllDepartamentoByEstudiante(User.GetUserId());
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            return Ok(resultado.Data);
        }
    }
}