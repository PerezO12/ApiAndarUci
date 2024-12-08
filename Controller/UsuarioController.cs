using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
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
                if(!ModelState.IsValid) return BadRequest(new {msg = "Modelo no valido"});

                var resultado = await _usuarioService.UpdateAsync(id, usuarioUpdateDto);

                if(!resultado.Succeeded) return NotFound(new {msg = resultado.Errors.Select(e => e.Description).ToArray()});

                return Ok(new {msg=  "Usuario actualizado"});
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, new {msg = "Algo salio mal, notificar al administrador"});
            }
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromBody]PasswordDto password) 
        {
            if(!ModelState.IsValid) return BadRequest(new {msg = "La contraseña es obligatoria"});
            var adminId = User.FindFirst("UsuarioId")?.Value;
            if(adminId == null) return BadRequest("Token no valido");
            try
            {
                var resultado = await _usuarioService.DeleteUserYRolAsync(id, adminId, password.Password);
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
                Console.Write(ex);
                return StatusCode(500, new {msg = "Ocurrio un error, informar al administrador"});
            }
        }                 
    }
}