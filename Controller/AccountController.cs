using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
//Crear una ruta donde sea validar jwt y retornar datos del suaurio con ese jwt
namespace MyApiUCI.Controller
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _acountService;


        public AccountController(IAccountService acountService, ApplicationDbContext context, ITokenService tokenService)
        {
            _acountService = acountService;
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("registrar/admin")]
        public async Task<IActionResult> RegistrarAdmin([FromBody] RegistroAdministradorDto registroDto) 
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new {msg = ModelState.ToArray()});
            }
            try
            {
                //registrar al usuario
                var (resultado, newAdminDto) = await _acountService.RegistrarAdministradorAsync(registroDto);
                if(!resultado.Succeeded) return BadRequest(new {msg = resultado.Errors.ToArray()});
                
                return StatusCode(201, newAdminDto);

            } catch(Exception ex) 
            {
                Console.Write(ex.Message);
                return StatusCode(500, new { message = "Contactar al administrador" });
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("registrar/estudiante")]
        public async Task<IActionResult> RegisterEstudiante([FromBody] RegisterEstudianteDto registerDto)
        {
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(new {msg = ModelState.ToArray()});
            }

            try
            {
                // Llamar al servicio para registrar al usuario
                var (resultRegister, newEstudianteDto) = await _acountService.RegisterEstudianteAsync(registerDto);
                
                if (!resultRegister.Succeeded) return BadRequest(new {msg = resultRegister.Errors.ToArray()});

                return StatusCode(201, newEstudianteDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = ex.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("registrar/encargado")]
        public async Task<IActionResult> RegisterEncargado([FromBody] RegisterEncargadoDto registerDto)
        {
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(new {msg = ModelState});
            }

            try
            {
                // Llamar al servicio para registrar al usuario
                var (resultRegister, newEncargadoDto) = await _acountService.RegisterEncargadoAsync(registerDto);
                
                if (!resultRegister.Succeeded) return BadRequest(new {msg = resultRegister.Errors});

                return Ok(newEncargadoDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = ex.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {

                if(!ModelState.IsValid) return BadRequest(new{ msg = "Contraseña/Usuario incorrecto"});//NO HACe falta

                var user = await  _acountService.Login(loginDto);
                
                if(user == null) return Unauthorized(new { msg = "Usuario o Contraseña Incorrectos"});

                return Ok(user);
            }
            catch(Exception ex)
            {
                Console.Write(ex);
                return StatusCode(500, new {msg="Ocurrio un error, informar al administrador"});
            }
            
        }

        [Authorize]
        [HttpGet("obtener-perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            //solo usuarios con token validospodran acceder
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null) return BadRequest(new {msg = "Token no valido"});

            var usuarioDto = await _acountService.ObtenerPerfilAsync(userId);
            return Ok(usuarioDto);
        }
        
        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto cuentadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("UsuarioId")?.Value;
            if(userId == null) return BadRequest(new {msg = "Token no valido"});

            try
            {
                // Llamar al servicio para registrar al usuario
                var resultado = await _acountService.CambiarPasswordAsync(userId, cuentadDto );
                
                if (!resultado.Succeeded) return BadRequest(new {msg = resultado.Errors});

                return Ok(new {msg = "La contraseña fue cambiada exitosamente"});

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = ex.Message });
            }
        }
    }
}
