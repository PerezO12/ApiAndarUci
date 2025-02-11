using ApiUci.Contracts.V1;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Usuarios;
using ApiUci.Extensions;
using ApiUci.Interfaces;
using ApiUci.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiUci.Helpers;

namespace ApiUci.Controller
{   
    [Authorize(Policy = "AdminPolicy")]
    [Route(ApiRoutes.Usuario.RutaGenaral)]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;
        public UsuarioController(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost(ApiRoutes.Usuario.RegistrarAdmin)]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistroAdministradorDto registroDto) 
        {

/*             var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), registroDto.PasswordAdmin);
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);
            } */
            var resultado = await _usuarioService.RegistrarAdministradorAsync(registroDto);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return CreatedAtAction(nameof(GetById), new { id = resultado.Data?.Id }, resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectUsuario query)
        {
            var resultado = await _usuarioService.GetAllAsync(query);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute]string Id)
        {
            var resultado = await _usuarioService.GetByIdAsync(Id);
            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
        
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
/* 
            var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), usuarioUpdateDto.PasswordAdmin );
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);
            }
 */
            var resultado = await _usuarioService.UpdateAsync(id, usuarioUpdateDto);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromBody]PasswordDto password) 
        {
            var passwordResult = await _authService.VerifyUserPassword(User.GetUserId(), password.Password );
            if (!passwordResult)
            {
                var error = ErrorBuilder.Build("Password", "Contraseña incorrecta.");
                return ActionResultHelper.HandleActionResult("Unauthorized", error);
            }

            var resultado = await _usuarioService.DeleteUserYRolAsync(id);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }                 
    }
}