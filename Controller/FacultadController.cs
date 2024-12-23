using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Dtos.Facultad;
using Microsoft.AspNetCore.Authorization;
using ApiUCI.Helpers.Querys;
using ApiUCI.Interfaces;
using ApiUCI.Dtos.Cuentas;
using System.Security.Claims;
using ApiUCI.Contracts.V1;



namespace MyApiUCI.Controller
{
    [Route(ApiRoutes.Facultad.RutaGenaral)]
    [ApiController]
    public class FacultadController : ControllerBase
    {
        private readonly IFacultadRepository _facultadRepo;
        private readonly IAuthService _authService;
        private readonly IDepartamentoService _depaService;
        public FacultadController( IFacultadRepository facultadRepo, IAuthService authService, IDepartamentoService depaService)
        {
            _facultadRepo = facultadRepo;
            _authService = authService;
            _depaService = depaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectFacultad query)
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

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Created(FacultadCreateDto facultadDto)
        {

            var facultadModel = facultadDto.toFacultadFromCreate();

            await _facultadRepo.CreateAsync(facultadModel);
            
            return CreatedAtAction(nameof(GetById), new { id = facultadModel.Id }, facultadModel.toFacultadDto());
        }

        [Authorize(Policy = "AdminPolicy")]
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
       /*  
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id, [FromBody] PasswordDto password) {

            try
            {
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new {msg="Token no válido"});
                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return BadRequest(new {msg="El usuario no existe"});
                var passwordCorrecta = await _authService.VerifyUserPassword(admin, password.Password);
                if(!passwordCorrecta) return Unauthorized(new {msg="La contraseña es incorrecta"});

                var facultadModel = await _facultadRepo.DeleteAsync(id);
                if(facultadModel == null) return NotFound("La facultad no existe");
                //borrar sus respectivos departamentos
                await _depaService.DeleteAllDepartamentosByFacultad(facultadModel.Id);
                //asignarle a sus respectivas carreras un null

                return Ok(new {msg= "Facultad eliminada exitosamente"} );
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Ocurrió un error, contactar al administrador");
            }
        }  */

   
    }
}