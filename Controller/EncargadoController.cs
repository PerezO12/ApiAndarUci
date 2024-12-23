using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Contracts.V1;
using ApiUCI.Dtos.Encargado;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Controller
{
    [Route(ApiRoutes.Encargado.RutaGenaral)]
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

        [Authorize(Policy = "AdminPolicy")]
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
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var encargadoDto = await _encargadoService.GetByIdEncargadoWithDetailsAsync(id);
            if(encargadoDto == null) return NotFound("No existe el encargado");

            return Ok(encargadoDto);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet(ApiRoutes.Encargado.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id)
        {
            var encargado = await _encargadoService.GetByUserIdWithUserId(id);
            if(encargado == null) return NotFound(new {msg = "El encargado no existe"});

            return Ok(encargado);
        }

        //Generar sus llaves publicas llaves privadas solo acceder encargados
        [HttpPost(ApiRoutes.Encargado.CambiarLlaves)]
        public async Task<IActionResult> CambiarLlavePublica([FromBody] EncargadoCambiarLlaveDto encargado) {
            if(!ModelState.IsValid) return  BadRequest(new {msg="No válido"});
            try
            {
                var usuarioId = User.FindFirst("UsuarioId")?.Value;
                if (usuarioId == null)
                {
                    return BadRequest(new {msg="Token no válido."});
                }
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if(usuario == null ) return NotFound(new {msg="Usuario no existe"}); 

                var result = await _signInManager.CheckPasswordSignInAsync(usuario, encargado.Password, false);
                if(!result.Succeeded) return NotFound(new {msg="Contraseña incorrecta"});

                var llaves = await _encargadoService.CambiarLlavePublicalAsync(usuarioId, encargado);
                
                if(llaves == null) return BadRequest(new {msg="clave pública no válida"});
                
                return Ok(llaves);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Contactar al administrador");
            }
        }

        [HttpPost(ApiRoutes.Encargado.GenerarLlaves)]
        public async Task<IActionResult> GenerarLlaves([FromBody] EncargadoGenerarLLaveDto encargado) {
            if(!ModelState.IsValid) return  BadRequest(new {msg="No válido"});
            
            try{
                var usuarioId = User.FindFirst("UsuarioId")?.Value;
                if (usuarioId == null)
                {
                    return BadRequest(new {msg="Token no válido."});
                }
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if(usuario == null ) return NotFound(new {msg="Usuario no existe"}); 

                var result = await _signInManager.CheckPasswordSignInAsync(usuario, encargado.Password, false);
                if(!result.Succeeded) return NotFound(new {msg="Usuario/Password incorrectos"});

                var llaves = await _encargadoService.GenerarFirmaDigitalAsync(usuarioId);
                
                if(llaves == null) return StatusCode(500, "Contactar al administrador");

                return Ok(llaves);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new {msg= "Contactar al administrador"});
            }
        }
    }
}