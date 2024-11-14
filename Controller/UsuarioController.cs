using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioRepository usuarioRepo, IUsuarioService usuarioService)
        {
            _usuarioRepo = usuarioRepo;
            _usuarioService = usuarioService;
        }
        
        //[Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectUsuario query)
        {
            var usuarios = await _usuarioRepo.GetAllAsync(query);
            var usuariosDto = usuarios.Select( u => u.toUsuarioDto());
            return Ok(usuariosDto);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute]string Id)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(Id);
            if(usuario == null) return NotFound("No existe el usuario");
            return Ok(usuario);
        }
        //agregar proteccion de administrador
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UsuarioUpdateDto usuarioUpdateDto)
        {
            if(!ModelState.IsValid) return BadRequest("Modelo no valido");

            var resultado = await _usuarioService.UpdateAsync(id, usuarioUpdateDto);

            if(!resultado.Succeeded) return NotFound(resultado.Errors.Select(e => e.Description).ToList());

            if(resultado.Succeeded) return Ok("Usuario actualizado");

            return StatusCode(500,"Algo salio mal notificar al backend");
        }                  
    }
}