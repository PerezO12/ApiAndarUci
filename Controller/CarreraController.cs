using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Carrera;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarreraController : ControllerBase
    {
        private readonly ICarreraRepository _carreraRepo;
        private readonly IFacultadRepository _facuRepo;

        public CarreraController( ICarreraRepository carreraRepo, IFacultadRepository facuRepo)
        {
            _carreraRepo = carreraRepo;
            _facuRepo = facuRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var carreraModel = await _carreraRepo.GetAllAsync(query);
            var carreraDto = carreraModel.Select( c => c.toCarreraDto());

            return Ok(carreraDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            var carrera = await _carreraRepo.GetByIdAsync(id);
            
            if(carrera == null) return NotFound("Carrera no existe");


            return Ok(carrera.toCarreraDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarreraDto carreraDto)
        {
            if(!ModelState.IsValid) return BadRequest("El modelo no es valido");

            if( !await _facuRepo.FacultyExists(carreraDto.FacultadId)) return NotFound("Facultad no existe");

            var carreraModel = await _carreraRepo.CreateAsync(carreraDto.toCarreraFromCreate());

            return CreatedAtAction(nameof(GetByID), new{Id = carreraModel.Id}, carreraModel.toCarreraDto());

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id ,[FromBody] UpdateCarreraDto carreraDto)
        {
            if(!ModelState.IsValid) return BadRequest("El modelo no es valido");
            
            if( !await _facuRepo.FacultyExists(carreraDto.FacultadId)) return NotFound("Facultad no existe");

            var carreraModel = await _carreraRepo.UpdateAsync(id, carreraDto.toCarreraFromUpdate());
            
            if(carreraModel == null) return NotFound("Esta carrera no existe");

            return Ok(carreraModel.toCarreraDto());
            
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var carreraModel = await _carreraRepo.DeleteAsync(id);
            if(carreraModel == null) return NotFound("La carrera no existe");
            
            return Ok(carreraModel.toCarreraDto());
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchCarreraDto carreraDto)
        {
            if(!ModelState.IsValid) return BadRequest("El modelo no es válido");

            var carrera = await _carreraRepo.PatchAsync(id, carreraDto);
            
            if(carrera == null) return NotFound("La carrera no existe");
            
            return Ok(carrera.toCarreraDto());
        }
    }
}