using ApiUCI.Contracts.V1;
using ApiUCI.Interfaces;
using ApiUCI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUCI.Dtos.Carrera;
using ApiUCI.Helpers;


namespace ApiUCI.Controller
{
    [Route(ApiRoutes.Carrera.RutaGenaral)]
    [ApiController]
    public class CarreraController : ControllerBase
    {
        private readonly ICarreraService _carreraService;

        public CarreraController( ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectCarrera query)
        {
            var resultado = await _carreraService.GetAllAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
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