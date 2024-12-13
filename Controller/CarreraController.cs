using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectCarrera query)
        {
            try
            {
                var carreraModel = await _carreraRepo.GetAllAsync(query);
                var carreraDto = carreraModel.Select( c => c.toCarreraDto());

                return Ok(carreraDto);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al obtener las Carreras, contacte al administrador" });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            try
            {
                var carrera = await _carreraRepo.GetByIdAsync(id);
                
                if(carrera == null) return NotFound("Carrera no existe");


                return Ok(carrera.toCarreraDto());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al obtener la carrera, informe al administrador" });
            }
        }

        //[Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarreraDto carreraDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest("El modelo no es válido");

                if( !await _facuRepo.FacultyExists(carreraDto.FacultadId)) return NotFound("Facultad no existe");

                var carreraModel = await _carreraRepo.CreateAsync(carreraDto.toCarreraFromCreate());

                return CreatedAtAction(nameof(GetByID), new{Id = carreraModel.Id}, carreraModel.toCarreraDto());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al crear la carrera, informe al administrador" });
            }

        }
        //[Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id ,[FromBody] UpdateCarreraDto carreraDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest("El modelo no es válido");
                
                if( !await _facuRepo.FacultyExists(carreraDto.FacultadId)) return NotFound("Facultad no existe");

                var carreraModel = await _carreraRepo.UpdateAsync(id, carreraDto.toCarreraFromUpdate());
                
                if(carreraModel == null) return NotFound("Esta carrera no existe");
                
                return Ok(carreraModel.toCarreraDto());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al actualizar la carrera, informe al administrador" });
            }
            
        }
        //[Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var carreraModel = await _carreraRepo.DeleteAsync(id);
                if(carreraModel == null) return NotFound("La carrera no existe");
                
                return Ok(carreraModel.toCarreraDto());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al borrar la carrera, informe al administrador" });
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch([FromRoute] int id, [FromBody] PatchCarreraDto carreraDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest("El modelo no es válido");

                var carrera = await _carreraRepo.PatchAsync(id, carreraDto);
                
                if(carrera == null) return NotFound("La carrera no existe");
                
                return Ok(carrera.toCarreraDto());
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { msg = "Error al actualizar la carrera, informe al administrador" });
            }
        }
    }
}