using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Dtos.Facultad;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Helpers;


namespace MyApiUCI.Controller
{
    [Route("api/[Controller]")]
    [ApiController]
    public class FacultadController : ControllerBase
    {
        private readonly IFacultadRepository _facultadRepo;
        public FacultadController( IFacultadRepository facultadRepo)
        {
            _facultadRepo = facultadRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var facultad = await _facultadRepo.GetAllAsync(query);
            var facultadDto = facultad.Select( f => f.toFacultadDto());
            
            return Ok(facultadDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var facultadModel = await _facultadRepo.GetByIdAsync(id);
            if(facultadModel == null)
            {
                return NotFound("Faculty does not exist");
            }
            return Ok(facultadModel.toFacultadDto()); //convertir a dto
        }
        [HttpPost]
        public async Task<IActionResult> Created(FacultadCreateDto facultadDto)
        {

            var facultadModel = facultadDto.toFacultadFromCreate();

            await _facultadRepo.CreateAsync(facultadModel);
            
            return CreatedAtAction(nameof(GetById), new { id = facultadModel.Id }, facultadModel.toFacultadDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody]FacultadUpdateDto facultadDto)
        {

            var facultadModel = await _facultadRepo.UpdateAsync(id, facultadDto.toFacultadFromUpdate());
            
            if(facultadModel == null)
            {
                return NotFound("Faculty not found");
            }

            return Ok(facultadModel.toFacultadDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id) {
            var facultadModel = await _facultadRepo.DeleteAsync(id);
            
            if(facultadModel == null)
            {
                return NotFound("Faculty not found");
            }
            return Ok(facultadModel.toFacultadDto());
        } 

   
    }
}