using ApiUci.Contracts.V1;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Encargado;
using ApiUci.Extensions;
using ApiUci.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUci.Helpers;
using ApiUci.Interfaces;

namespace ApiUci.Controller
{
    [Route(ApiRoutes.Encargado.RutaGenaral)]
    [ApiController]
    public class EncargadoController : ControllerBase
    {
        private readonly IEncargadoService _encargadoService;

        public EncargadoController(IEncargadoService encargadoService)
        {
            _encargadoService = encargadoService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Encargado.RegistrarEncargado)]
        public async Task<IActionResult> RegisterEncargado([FromBody] RegisterEncargadoDto registerDto)
        {
            var resultado = await _encargadoService.RegisterEncargadoAsync(registerDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return CreatedAtAction(nameof(GetById), new {id = resultado?.Data?.Id}, resultado?.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEncargado query)
        {
            var resultado = await _encargadoService.GetAllEncargadosWithDetailsAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var resultado = await _encargadoService.GetByIdEncargadoWithDetailsAsync(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet(ApiRoutes.Encargado.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id)
        {
            var resultado = await _encargadoService.GetByUserIdWithUserId(id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "EncargadoPolicy")]
        [HttpPost(ApiRoutes.Encargado.CambiarLlaves)]
        public async Task<IActionResult> CambiarLlavePublica([FromBody] EncargadoCambiarLlaveDto encargado)
        {
            var resultado = await _encargadoService.CambiarLlavePublicalAsync(User.GetUserId(), encargado);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [HttpPost(ApiRoutes.Encargado.GenerarLlaves)]
        public async Task<IActionResult> GenerarLlaves([FromBody] PasswordDto password)
        {
            var resultado = await _encargadoService.GenerarFirmaDigitalAsync(User.GetUserId(), password);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }
    }
}
