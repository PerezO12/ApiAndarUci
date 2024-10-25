using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;
using MyApiUCI.Repository;

namespace MyApiUCI.Controller
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DepartamentoController : ControllerBase 
    {
        private readonly IDepartamentoRepository _depaRepo;
        private readonly IFacultadRepository _facuRepo;
        public DepartamentoController(IDepartamentoRepository depaRepo, IFacultadRepository facuRepo)
        {
            _depaRepo = depaRepo;
            _facuRepo = facuRepo;
        }

        //Get
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departamentoModel = await _depaRepo.GetAllAsync();
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
                return NotFound("Departament does not exist");
            }
            return Ok(departamentoModel.toDepartamentDto()); 
        }


        //Post
        [HttpPost]
        public async Task<IActionResult> Create( [FromBody]CreateDepartamentoDto departamentoDto)
        {
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
            if( !await _facuRepo.FacultyExists(departamentoDto.FacultadId))
            {
               return NotFound("Faculty does not exist");
            }

            var departamentoModel = await _depaRepo.UpdateAsync(id, departamentoDto.toDepartamentoFromUpdate());
            
            if(departamentoModel == null)
            {
                return NotFound("Departament does not exist");
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
                return NotFound("Departament does not exist");
            }
            return Ok(departamentoModel.toDepartamentDto());
        }

    }
}