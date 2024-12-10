
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Helpers.Querys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;
using MyApiUCI.Repository;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentoController : ControllerBase 
    {
        private readonly IDepartamentoRepository _depaRepo;
        private readonly IFacultadRepository _facuRepo;
        private readonly IEstudianteService _estudianteService;
        private readonly IEncargadoService _encargadoService;
        private readonly ILogger<DepartamentoController> _logger;
        public DepartamentoController(
            IDepartamentoRepository depaRepo,
            IFacultadRepository facuRepo,
            IEstudianteService estudianteService,
            IEncargadoService encargadoService,
            ILogger<DepartamentoController> logger
            )
        {
            _depaRepo = depaRepo;
            _facuRepo = facuRepo;
            _encargadoService = encargadoService;
            _estudianteService = estudianteService;
            _logger = logger;
        }

        //Get
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll( [FromQuery] QueryObjectDepartamentos query)
        {
            try
            {
                var departamentoModel = await _depaRepo.GetAllAsync(query);
                //convertir a deparamentDTOP
                var departamentoDto = departamentoModel.Select(d => d.toDepartamentDto());

                return Ok(departamentoDto);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener los departamentos: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al obtener los departamentos. Informar al administrador"});
            }
        }

        //GetByID
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try{
            var departamentoModel = await _depaRepo.GetByIdAsync(id);
            if(departamentoModel == null)
            {
                return NotFound(new {msg="Departament no existe"});
            }
            var encargado = await _encargadoService.GetEncargadoByDepartamentoIdAsync(departamentoModel.Id);

            return Ok(departamentoModel.toDepartamentDto()); 
                
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener el departamento {id}: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al obtener el departamento. Informar al administrador"});
            }
        }

        //Post
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create( [FromBody]CreateDepartamentoDto departamentoDto)
        {
            try{
                if(!ModelState.IsValid) return BadRequest("El modelo no es válido");
                if( !await _facuRepo.FacultyExists(departamentoDto.FacultadId))
                {
                return NotFound("Faculty does not exist");
                }
                var departamentoModel = departamentoDto.toDepartamentoFromCreate();
                await _depaRepo.CreateAsync(departamentoModel);

                return CreatedAtAction(nameof(GetById), new {id = departamentoModel.Id}, departamentoModel.toDepartamentDto());
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al crear el departamento: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al crear el departamento. Informar al administrador"});
            }
        }

        //Put
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateDepartamentoDto departamentoDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest("El modelo no es válido");
                if( !await _facuRepo.FacultyExists(departamentoDto.FacultadId))
                {
                return NotFound("Facultad no existe");
                }

                var departamentoModel = await _depaRepo.UpdateAsync(id, departamentoDto.toDepartamentoFromUpdate());
                
                if(departamentoModel == null)
                {
                    return NotFound("Departamento no existe");
                }
                
                return Ok(departamentoModel.toDepartamentDto());

            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar el departamento{id}: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al actualizar el departamento. Informar al administrador"});
            }
        }

        //Delete
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                var departamentoModel = await _depaRepo.DeleteAsync(id);
                if(departamentoModel == null)
                {
                    return NotFound("El departamento no existe");
                }
                //cuando tu borras uun departamento debes borrar el encargado
                await _encargadoService.DeleteEncargadoByDepartamentoIdAsync(departamentoModel.Id, false);

                return Ok(new {msg="Departamento borrado exitosamente"});
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al borrar el departamento{id}: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al borrar el departamento. Informar al administrador"});
            }
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchDepartamentoDto departamentoDto)
        {
            try
            {
                var departamentoModel = await _depaRepo.PatchAsync(id, departamentoDto);
                if(departamentoModel == null)
                {
                    return NotFound("Departament does not exist");
                }
                return Ok(departamentoModel.toDepartamentDto());
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar el departamento {id}: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al actualizar el departamento. Informar al administrador"});
            }

        }
        //estudiantes
        [HttpGet("correspondientes")]
        public async Task<IActionResult> GetAllDepartamentosByEstudiante()
        {
            try 
            {
                var userId = User.FindFirst("UsuarioId")?.Value;
                if(userId == null) return BadRequest("Token no válido");

                var estudiante = await _estudianteService.GetEstudianteByUserId(userId);
                if(estudiante == null) return BadRequest("El estudiante no existe");

                var departamentos = await _depaRepo.GetAllDepartamentosByFacultadId(estudiante.FacultadId);
            
                if(departamentos == null) return NotFound("No hay departamentos correspondientes");
                var departamentosDto = departamentos.Select( d => d.toDepartamentDto());

                return Ok(departamentosDto);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener los departamentos: {ex.Message}");
                return StatusCode(500, new{ msg = "Error al obtener los departamentos. Informar al administrador"});
            }
        }
    }
}