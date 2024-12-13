using System.Security.Claims;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MyApiUCI.Helpers;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
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
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectUsuario query)
        {
            var usuariosDto = await _usuarioService.GetAllAsync(query);
            return Ok(usuariosDto);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute]string Id)
        {
            var usuario = await _usuarioService.GetByIdAsync(Id);
            if(usuario == null) return NotFound("No existe el usuario");
            return Ok(usuario);
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            try 
            {
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new{msg="Token no válido"});

                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return Unauthorized(new {msg="No Authorizado"});

                var passwordCorrecta = await _authService.VerifyUserPassword(admin, usuarioUpdateDto.PasswordAdmin );
                if(!passwordCorrecta) return Unauthorized(new {msg="Contraseña incorrecta"});

                var resultado = await _usuarioService.UpdateAsync(id, usuarioUpdateDto);

                if(!resultado.Succeeded) return NotFound(new {msg = resultado.Errors.Select(e => e.Description).ToArray()});

                return Ok(new {msg=  "Usuario actualizado"});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new {msg = "Algo salio mal, notificar al administrador"});
            }
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromBody]PasswordDto password) 
        {
            if(!ModelState.IsValid) return BadRequest(new {msg = "La contraseña es obligatoria"});
            try
            {
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new{msg="Token no válido"});

                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return Unauthorized(new {msg="No Authorizado"});

                var passwordCorrecta = await _authService.VerifyUserPassword(admin, password.Password );
                if(!passwordCorrecta) return Unauthorized(new {msg="Contraseña incorrecta"});
                
                var resultado = await _usuarioService.DeleteUserYRolAsync(id, adminId);
                if(resultado.Error)
                {
                    return resultado.TipoError switch
                    {
                        "NotFound" => NotFound(new { msg = resultado.msg}),
                        "BadRequest" => BadRequest(new { msg = resultado.msg}),
                        "Unauthorized" => Unauthorized(new { msg = resultado.msg}),
                        _ => StatusCode(500, new { admin = "Falta validar esta acción. Reportar el error", msg = resultado.msg })
                    };
                }
                return Ok(new {msg = resultado.msg});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new {msg = "Ocurrió un error, informar al administrador"});
            }
        }                 
    }
}