using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;

namespace MyApiUCI.Controller
{
    [Route("api/account")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly IAcountService _acountService;

        public AcountController(IAcountService acountService, ApplicationDbContext context, ITokenService tokenService)
        {
            _acountService = acountService;
        }

        //[Authorize(Policy = "AdminPolicy")]
        [HttpPost("register/estudiante")]
        public async Task<IActionResult> RegisterEstudiante([FromBody] RegisterEstudianteDto registerDto)
        {
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Llamar al servicio para registrar al usuario
                var (resultRegister, newEstudianteDto) = await _acountService.RegisterEstudiante(registerDto);
                
                if (!resultRegister.Succeeded) return BadRequest(resultRegister.Errors);

                return Ok(newEstudianteDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = ex.Message });
            }
        }

        //[Authorize(Policy = "AdminPolicy")]
        [HttpPost("register/encargado")]
        public async Task<IActionResult> RegisterEncargado([FromBody] RegisterEncargadoDto registerDto)
        {
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Llamar al servicio para registrar al usuario
                var (resultRegister, newEncargadoDto) = await _acountService.RegisterEncargado(registerDto);
                
                if (!resultRegister.Succeeded) return BadRequest(resultRegister.Errors);

                return Ok(newEncargadoDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid) return BadRequest("Password/Usuario incorrecto");//NO HACe falta

            var user = await  _acountService.Login(loginDto);
            
            if(user == null) return Unauthorized("Usuario o Password Incorrectos");

            return Ok(user);
            
        }
    }
}
