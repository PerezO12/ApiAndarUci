using ApiUci.Contracts.V1;
using ApiUci.Dtos.Cuentas;
using ApiUci.Extensions;
using ApiUci.Interfaces;
using ApiUci.Utilities;
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

        [HttpPost(ApiRoutes.Account.Login)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var resultado = await _authService.Login(loginDto);

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
    }
}