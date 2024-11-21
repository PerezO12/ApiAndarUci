
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public DepartamentoController(
            IDepartamentoRepository depaRepo,
            IFacultadRepository facuRepo,
            IEstudianteService estudianteService
            )
        {
            _depaRepo = depaRepo;
            _facuRepo = facuRepo;
            _estudianteService = estudianteService;
        }

        //Get
        [HttpGet]
        public async Task<IActionResult> GetAll( [FromQuery] QueryObject query)
        {
            var departamentoModel = await _depaRepo.GetAllAsync(query);
            //convertir a deparamentDTOP
            var departamentoDto = departamentoModel.Select(d => d.toDepartamentDto());

            return Ok(departamentoDto);
        }

        //GetByID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var departamentoModel = await _depaRepo.GetByIdAsync(id);
            if(departamentoModel == null)
            {
                return NotFound("Departament no existe");
            }
            return Ok(departamentoModel.toDepartamentDto()); 
        }


        //Post
        //[Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create( [FromBody]CreateDepartamentoDto departamentoDto)
        {
            if(!ModelState.IsValid) return BadRequest("El modelo no es valido");
            if( !await _facuRepo.FacultyExists(departamentoDto.FacultadId))
            {
               return NotFound("Faculty does not exist");
            }
            var departamentoModel = departamentoDto.toDepartamentoFromCreate();
            await _depaRepo.CreateAsync(departamentoModel);

            return CreatedAtAction(nameof(GetById), new {id = departamentoModel.Id}, departamentoModel.toDepartamentDto());
        }

        //Put
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateDepartamentoDto departamentoDto)
        {
            if(!ModelState.IsValid) return BadRequest("El modelo no es valido");
            if( !await _facuRepo.FacultyExists(departamentoDto.FacultadId))
            {
               return NotFound("Facultad no existe");
            }

            var departamentoModel = await _depaRepo.UpdateAsync(id, departamentoDto.toDepartamentoFromUpdate());
            
            if(departamentoModel == null)
            {
                return NotFound("Departamento no existe");
            }
            
            return Ok(departamentoDto);
        }

        //Delete
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var departamentoModel = await _depaRepo.DeleteAsync(id);
            if(departamentoModel == null)
            {
                return NotFound("El departamento no existe");
            }
            return Ok(departamentoModel.toDepartamentDto());
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchDepartamentoDto departamentoDto)
        {
            var departamentoModel = await _depaRepo.PatchAsync(id, departamentoDto);
            if(departamentoModel == null)
            {
                return NotFound("Departament does not exist");
            }
            return Ok(departamentoModel.toDepartamentDto());
        
        }
        //estudiantes
        [HttpGet("correspondientes")]
        public async Task<IActionResult> GetAllDepartamentosCorrespondientes()
        {
            var userId = User.FindFirst("UsuarioId")?.Value;
            if(userId == null) return BadRequest("Token no valido");

            var estudiante = await _estudianteService.GetEstudianteByUserId(userId);
            if(estudiante == null) return BadRequest("El estudiante no existe");

            var departamentos = await _depaRepo.GetAllByFacultadId(estudiante.FacultadId);

            if(departamentos == null) return NotFound("No hay departamentos correspondientes");

            return Ok(departamentos);
        }
    }
}