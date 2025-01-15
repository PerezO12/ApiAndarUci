using ApiUci.Contracts.V1;
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Extensions;
using ApiUci.Interfaces;
using ApiUci.Utilities;
using ApiUCI.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiUci.Controller
{
    [Route(ApiRoutes.Account.RutaGenaral)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        //Metodo para loguear, y verifica si tienes 2fa activo
        [HttpPost(ApiRoutes.Account.Login)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var ipAddress = HttpContext.Items["ClientIp"]?.ToString();//el ip se add en el middleware
            if(ipAddress == null)
                return BadRequest(ErrorBuilder.Build("ipAddress", "No se pudo obtener la dirección IP"));

            var resultado = await _authService.Login(loginDto, ipAddress);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);

            return Ok(resultado.Data);
        }

        [Authorize]
        [HttpGet(ApiRoutes.Account.ObtenerPerfil)]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var resultado = await _authService.ObtenerPerfilAsync(User.GetUserId());

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
                
            return Ok(resultado.Data);

        }

        [Authorize]
        [HttpPost(ApiRoutes.Account.CambiarPassword)]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto cuentadDto)
        {
            var resultado = await _authService.CambiarPasswordAsync(User.GetUserId(), cuentadDto);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);  
        }
        [Authorize]
        [HttpPost(ApiRoutes.Account.Logout)]
        public async Task<IActionResult> Logout()
        {
            var resultado = await _authService.LogoutAsync(User.GetUserId());

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }
        //Genera el codigo QR para la autenticación de dos factores
        [Authorize]
        [HttpPost(ApiRoutes.Account.Generar2Fa)]
        public async Task<IActionResult> GenerateTwoFactorAuth()
        {
            var resultado = await _authService.GenerarTwoFactorAuthAsync(User.GetUserId(), Request);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        //Habilita la autenticación de dos factores y resive el primer codigo
        [Authorize]
        [HttpPost(ApiRoutes.Account.Enable2Fa)]
        public async Task<IActionResult> EnableTwoFactorAuth([FromBody] Code2Fa code2Fa)
        {
            var resultado = await _authService.EnableTwoFactorAuthAsync(User.GetUserId(), code2Fa.Code);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        //Valida el codigo y retorna el token de autenticación
        [Authorize]
        [HttpPost(ApiRoutes.Account.Validar2Fa)]
        public async Task<IActionResult> ValidateTwoFactorAuth([FromBody] Code2Fa code2Fa)
        {
            var resultado = await _authService.ValidateTwoFactorAuthAsync(User.GetUserId(), code2Fa.Code);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }

        [Authorize]
        [HttpPost(ApiRoutes.Account.Desactivar2Fa)]
        public async Task<IActionResult> DesactivarDobleFactorAuth([FromBody] Code2Fa code2Fa)
        {
            var resultado = await _authService.DesactivarDobleFactorAsync(User.GetUserId(), code2Fa.Code);

            if(!resultado.Success)
                return ActionResultHelper.HandleActionResult(resultado.ActionResult, resultado.Errors);
            
            return Ok(resultado.Data);
        }
    }
}