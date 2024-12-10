using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Interfaces;
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
        private readonly IAuthService _authService;

        public AccountController(IAccountService acountService, IAuthService authService)
        {
            _acountService = acountService;
            _authService = authService;
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
                //validacion de contraseña
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new {msg = "Token no válido"});

                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return NotFound(new {msg = "El usuario no existe"});

                var passwordResult = await _authService.VerifyUserPassword(admin, registroDto.PasswordAdmin);
                if(!passwordResult) return Unauthorized(new {msg = "Contraseña Incorrecta"});
                
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
                //validacion de contraseña
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new {msg = "Token no válido"});

                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return NotFound(new {msg = "El usuario no existe"});
                
                var passwordResult = await _authService.VerifyUserPassword(admin, registerDto.PasswordAdmin);
                if(!passwordResult) return Unauthorized(new {msg = "Contraseña Incorrecta"});

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
            //required password
            

            try
            {
                //validacion de contraseña
                var adminId = User.FindFirstValue("UsuarioId");
                if(adminId == null) return BadRequest(new {msg = "Token no válido"});

                var admin = await _authService.ExisteUsuario(adminId);
                if(admin == null) return NotFound(new {msg = "El usuario no existe"});
                
                var passwordResult = await _authService.VerifyUserPassword(admin, registerDto.PasswordAdmin);
                if(!passwordResult) return Unauthorized(new {msg = "Contraseña Incorrecta"});

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

                var user = await  _authService.Login(loginDto);
                
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
            //solo usuarios con token válidospodran acceder
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null) return BadRequest(new {msg = "Token no válido"});

            var usuarioDto = await _authService.ObtenerPerfilAsync(userId);
            return Ok(usuarioDto);
        }
        
        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto cuentadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirst("UsuarioId")?.Value;
                if(userId == null) return BadRequest(new {msg = "Token no válido"});

                var usuario = await _authService.ExisteUsuario(userId);
                if(usuario == null) return NotFound(new {msg = "El usuario no existe"});
                // Llamar al servicio para registrar al usuario
                var resultado = await _authService.CambiarPasswordAsync(usuario, cuentadDto );
                
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
