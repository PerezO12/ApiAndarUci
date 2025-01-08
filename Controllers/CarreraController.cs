using ApiUci.Contracts.V1;
using ApiUci.Interfaces;
using ApiUci.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUci.Dtos.Carrera;
using ApiUci.Helpers;
using ApiUci.Extensions;
using ApiUci.Dtos.Cuentas;


namespace ApiUci.Controller
{
    [Route(ApiRoutes.Carrera.RutaGenaral)]
    [ApiController]
    public class CarreraController : ControllerBase
    {
        private readonly ICarreraService _carreraService;
        private readonly IAuthService _authService;

        public CarreraController( ICarreraService carreraService, IAuthService authService)
        {
            _carreraService = carreraService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectCarrera query)
        {
            var resultado = await _carreraService.GetAllAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
             var resultado = await _carreraService.GetByIdAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarreraDto carreraDto)
        {
            var resultado = await _carreraService.CreateAsync(carreraDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id ,[FromBody] UpdateCarreraDto carreraDto)
        {
            var resultado = await _carreraService.UpdateAsync(id, carreraDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromBody] PasswordDto password)
        {
            //TODO: hacer un filtro
            var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), password.Password);
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);
            }
            var resultado = await _carreraService.DeleteAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchCarreraDto carreraDto)
        {
            var resultado = await _carreraService.PatchAsync(id, carreraDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
    }
}