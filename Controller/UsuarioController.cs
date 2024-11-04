using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UsuarioController(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectUsuario query)
        {
            var usuarios = await _usuarioRepo.GetAllAsync(query);
            var usuariosDto = usuarios.Select( u => u.toUsuarioDto());
            return Ok(usuariosDto);
        }            
    }
}