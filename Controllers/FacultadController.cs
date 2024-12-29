using Microsoft.AspNetCore.Mvc;
using ApiUCI.Interfaces;
using ApiUCI.Dtos.Facultad;
using Microsoft.AspNetCore.Authorization;
using ApiUCI.Helpers.Querys;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Contracts.V1;
using ApiUCI.Utilities;
using ApiUCI.Extensions;

namespace ApiUCI.Controller
{
    [Route(ApiRoutes.Facultad.RutaGenaral)]
    [ApiController]
    public class FacultadController : ControllerBase
    {
        private readonly IFacultadService _facultadService;
        private readonly IAuthService _authService;
        public FacultadController( IFacultadService facultadService, IAuthService authService)
        {
            _facultadService = facultadService;
            _authService = authService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectFacultad query)
        {
            var resultado = await _facultadService.GetAllAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var resultado = await _facultadService.GetByIdAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Created(FacultadCreateDto facultadDto)
        {
            var resultado = await _facultadService.CreateAsync(facultadDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = resultado.Data?.Id }, resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody]FacultadUpdateDto facultadDto)
        {

            var resultado = await _facultadService.UpdateAsync(id, facultadDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id, [FromBody] PasswordDto password) {

            //TODO: hacer un filtro
            var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), password.Password);
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);
            }
            
            var resultado = await _facultadService.DeleteAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
    }
}