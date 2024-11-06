using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormularioController : ControllerBase
    {
        private readonly IFormularioService _formularioService;
        private readonly IFormularioRepository _formularioRepo;
        private readonly IDepartamentoRepository _departamentoRepo;
        public FormularioController(
            IFormularioService formularioService, 
            IFormularioRepository formularioRepo,
            IDepartamentoRepository departamentoRepo
            )
        {
            _formularioRepo = formularioRepo;
            _formularioService = formularioService;
            _departamentoRepo = departamentoRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectFormulario query)
        {
                //agregar lo q falta
            var formularios = await _formularioService.GetAllFormulariosWhithDetailsAsync(query);
            return Ok(formularios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var formulario = await _formularioService.GetFormularioWithDetailsAsync(id);
            if(formulario == null) return NotFound(new { message = "Formulario no encontrado" });
            
            return Ok(formulario);
        }

        //[Authorize(Policy = "EstudiantePolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormularioDto formularioDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            //FALTA VALIDAR QUE EL FORMULARIO NO EXISTA EN OTRO DEPARTAMENTO y q la facultad sea del estudiante*****************************
            //obtener id del token
            var UsuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UsuarioId");
            string? usuarioId = UsuarioIdClaim?.Value;
            if(usuarioId == null ) return BadRequest("El token no contiene el Id del usuario."); //Esto no tiene xq pasar es x si acaso
            
            var departamento = await _departamentoRepo.GetByIdAsync(formularioDto.DepartamentoId);
            if(departamento == null) return BadRequest("El departamento no existe");
            
            try
            {
                var formularioCreado = await _formularioRepo.CreateAsync(formularioDto.toFormularioFromCreate(usuarioId, departamento.FacultadId));
                
                if (formularioCreado == null)
                {
                    return NotFound("El usuario al que pertenece este formulario no existe.");
                }
                //Arreglar esto cuando cree el get
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = formularioCreado.Id },
                    new { message = "Formulario creado exitosamente", id = formularioCreado.Id });
            }
            catch (Exception ex)
            {
                // Registra el error (log)
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}