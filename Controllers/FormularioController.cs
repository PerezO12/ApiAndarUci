
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiUci.Dtos.Formulario;
using ApiUci.Helpers;
using ApiUci.Interfaces;
using ApiUci.Helpers.Querys;
using ApiUci.Contracts.V1;
using ApiUci.Extensions;
using ApiUci.Utilities;

namespace ApiUci.Controller
{
    [Route(ApiRoutes.Formulario.RutaGenaral)]
    [ApiController]
    public class FormularioController : ControllerBase
    {
        private readonly IFormularioService _formularioService;
        public FormularioController(
            IFormularioService formularioService)
        {
            _formularioService = formularioService;
        }

        [Authorize( Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectFormulario query)
        {
            var resultado = await _formularioService.GetAllFormulariosWhithDetailsAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            return Ok(resultado.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var resultado = await _formularioService.GetFormularioWithDetailsAsync(id);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            return Ok(resultado.Data);
        }
  
        [Authorize(Policy = "EstudiantePolicy")]
        [HttpGet(ApiRoutes.Formulario.GetFormularioEstudiante)]
        public async Task<IActionResult> GetFormulariosEstudiante([FromQuery] QueryObjectFormularioEstudiantes query){
            var resultado = await _formularioService.GetAllFormulariosEstudiantesAsync(User.GetUserId(), query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "EncargadoPolicy")]
        [HttpGet(ApiRoutes.Formulario.GetAllFormulariosByEncargado)]
        public async Task<IActionResult> GetAllFormulariosByEncargado([FromQuery] QueryObjectFormularioEncargado query)
        {
            var resultado = await _formularioService.GetAllFormulariosEncargadosAsync(User.GetUserId(), query);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            return Ok(resultado.Data);
        }

        [Authorize(Policy = "EncargadoPolicy")]
        [HttpGet(ApiRoutes.Formulario.GetFormEstudianteByIdForEncargadoAsync)]
        public async Task<IActionResult> GetFormEstudianteByIdForEncargadoAsync([FromRoute] int id)
        {
            var resultado = await _formularioService.GetFormEstudianteByIdForEncargadoAsync(User.GetUserId(), id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "EstudiantePolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormularioDto formularioDto)
        {

            var resultado = await _formularioService.CreateFormularioAsync(User.GetUserId(), formularioDto);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return CreatedAtAction(nameof(GetById), new { id = resultado.Data?.Id }, resultado.Data);
        }

        [Authorize(Policy = "EstudiantePolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody]UpdateFormularioDto formularioDto)
        {
            var resultado = await _formularioService.UpdatePatchFormularioAsync(User.GetUserId(), id, formularioDto);
        
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
                
            return Ok(resultado.Data);
        }

        //solo encargado
        [Authorize(Policy="EncargadoPolicy")]
        [HttpPatch(ApiRoutes.Formulario.FirmarFormulario)]
        public async Task<IActionResult> FirmarFormulario([FromRoute] int id, [FromBody] FormularioFirmarDto formularioDto)
        {
            var resultado = await _formularioService.FirmarFormularioAsync(User.GetUserId(), id, formularioDto);
                //Validaciones
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize(Policy ="EstudiantePolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormularioEstudiante([FromRoute]int id)
        {
            var resultado = await _formularioService.DeleteFormularioEstudianteAsync(User.GetUserId(), id);
        
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }


    }
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