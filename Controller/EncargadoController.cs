using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncargadoController : ControllerBase
    {
        private readonly IEncargadoService _encargadoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public EncargadoController(IEncargadoService encargadoService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _encargadoService = encargadoService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //[Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEncargado query)
        { 
            if (query.NumeroPagina <= 0)
            {
                return BadRequest("El número de página debe ser mayor que cero.");
            }

            if (query.TamañoPagina <= 0)
            {
                return BadRequest("El tamaño de la página debe ser mayor que cero.");
            }
           var encargados = await _encargadoService.GetAllEncargadosWithDetailsAsync(query);
           return Ok(encargados);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var encargadoDto = await _encargadoService.GetByIdEncargadoWithDetailsAsync(id);
            if(encargadoDto == null) return NotFound("No existe el encargado");

            return Ok(encargadoDto);
        }

        //Generar sus llaves publicas llaves privadas solo acceder encargados
        [HttpPost("cambiar-llave")]
        public async Task<IActionResult> CambiarLlavePublica([FromBody] EncargadoCambiarLlaveDto encargado) {
            if(!ModelState.IsValid) return  BadRequest("No valido");
            
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            if (usuarioId == null)
            {
                return BadRequest("Token no valido.");
            }
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if(usuario == null ) return NotFound("Usuario no existe"); 

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, encargado.Password, false);
            if(!result.Succeeded) return NotFound("Usuario/Password incorrectos");

            var llaves = await _encargadoService.CambiarLlavePublicalAsync(usuarioId, encargado);
            
            if(llaves == null) return BadRequest("Llave publica no valida");
            
            return Ok(llaves);
        }

        [HttpPost("generar-llaves")]
        public async Task<IActionResult> GenerarLlaves([FromBody] EncargadoGenerarLLaveDto encargado) {
            if(!ModelState.IsValid) return  BadRequest("No valido");
            
            
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            if (usuarioId == null)
            {
                return BadRequest("Token no valido.");
            }
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if(usuario == null ) return NotFound("Usuario no existe"); 

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, encargado.Password, false);
            if(!result.Succeeded) return NotFound("Usuario/Password incorrectos");

            var llaves = await _encargadoService.GenerarFirmaDigitalAsync(usuarioId);
            
            if(llaves == null) return BadRequest("Llave publica no valida");

            return Ok(llaves);
        }
    }
}