using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Extensions;
using ApiUci.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiUci.Mappers;
using ApiUci.Models;
using ApiUCI.Dtos.Auth;
using ApiUCI.Utilities;

namespace ApiUci.Service
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        public async Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto)
        {
            try
            {

                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.UserName.ToUpper());
                
                if (user == null)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Usuario/Contraseña","El nombre de usuario o la contraseña proporcionados no son correctos.");

                if(user.LockoutEnd != null)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Cuenta bloqueada", "La cuenta está temporalmente bloqueada debido a múltiples intentos fallidos. Intente nuevamente después de que finalice el bloqueo.");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
                                
                if (!result.Succeeded)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Usuario/Contraseña","El nombre de usuario o la contraseña proporcionados no son correctos.");
                    

                var roles = await _userManager.GetRolesAsync(user);
                
                if (roles == null || !roles.Any())
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Roles", "No se pudo obtener el perfil del usuario.", "Unauthorized");
                

                if(await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    var tempToken = _tokenService.CreateTemporaryTokenAsync(user);
                    var mewPerfilUserDto = user.toUserPerfilDto(roles, tempToken, true);
                    return RespuestasGenerales<UserPerfilDto>.SuccessResponse(mewPerfilUserDto);
                }
                var token = await _tokenService.CreateTokenAsync(user);
                await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);

                var newPerfilUserDto = user.toUserPerfilDto(roles, token);

                return RespuestasGenerales<UserPerfilDto>.SuccessResponse(newPerfilUserDto, "Sesión iniciada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine("**************************************************");
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string usuarioId)
        {
            try
            {
                var usuarioModel = await _userManager.FindByIdAsync(usuarioId);
                if (usuarioModel == null)
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("UsuarioId", "El usuario no existe.", "Unauthorized");

                var roles = await _userManager.GetRolesAsync(usuarioModel);
                if (roles == null || !roles.Any())
                    return RespuestasGenerales<UserPerfilDto>.ErrorResponseService("Roles", "No se pudo obtener el perfil del usuario.", "Unauthorized");
                var tieneDobleFactor = await _userManager.GetTwoFactorEnabledAsync(usuarioModel);
                var userPerfilDto = usuarioModel.toUserPerfilDto(roles,null, tieneDobleFactor );

                return RespuestasGenerales<UserPerfilDto>.SuccessResponse(userPerfilDto, "Perfil obtenido exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<bool> VerifyUserPassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
            
        public async Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if(usuario == null)
                    return RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe", "Unauthorized");
                var resultado = await _userManager.ChangePasswordAsync(usuario, cuentaDto.PasswordActual, cuentaDto.PasswordNueva);

                if (!resultado.Succeeded)
                {
                    var errors = ErrorBuilder.ParseIdentityErrors(resultado.Errors);
                    return RespuestasGenerales<bool>.ErrorResponseController(errors, "Error al cambiar la contraseña.");
                }

                return RespuestasGenerales<bool>.SuccessResponse(true, "Contraseña cambiada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<AppUser?> ExisteUsuario(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public Task<RespuestasGenerales<bool>> LogoutAsync(string userId)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                if (user == null)
                    return Task.FromResult(RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe", "Unauthorized"));

                var result = _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "AccessToken").Result;

                if (!result.Succeeded)
                    return Task.FromResult(RespuestasGenerales<bool>.ErrorResponseService("Token", "Error al cerrar sesión"));

                return Task.FromResult(RespuestasGenerales<bool>.SuccessResponse(true, "Sesión cerrada exitosamente"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<RespuestasGenerales<QrCodeUri>> GenerarTwoFactorAuthAsync(string userId, HttpRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return RespuestasGenerales<QrCodeUri>.ErrorResponseService("Usuario", "El usuario no existe.", "Unauthorized");

            //GENERAR clave secreta
            var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(secretKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            //uri para google authenticator
            var qrCodeUri = $"otpauth://totp/AndarUCI:{user.Email}?secret={secretKey}&issuer=AndarUCI";
            
            // Ruta para guardar la imagen en la carpeta "imagenes"
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "imagenes");
            // Verifica si la carpeta existe, si no la crea
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            // Nombre del archivo basado en el correo o nombre de usuario
            var fileName = $"qr_{RandomUuidGenerator.GenerateRandomUuid()}.png";
            var outputFilePath = Path.Combine(directoryPath, fileName);
    
            //generar código QR
            await QRCodeGenerator.GenerateQRCodeAsync(qrCodeUri, outputFilePath, 20);

            // Retorna la URL accesible desde el frontend
            var baseUrl = $"{request.Scheme}://{request.Host}/images"; 
            var qrUrl = $"{baseUrl}/{fileName}"; 
            return RespuestasGenerales<QrCodeUri>.SuccessResponse(new QrCodeUri { url = qrUrl }, "Código QR generado exitosamente.");
        }



        public async Task<RespuestasGenerales<bool>> EnableTwoFactorAuthAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe.", "Unauthorized");
        
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
            if (!isValid)
                return RespuestasGenerales<bool>.ErrorResponseService("Código", "Código de autenticación inválido.");
        
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!result.Succeeded)
            {
                var errors = ErrorBuilder.ParseIdentityErrors(result.Errors);
                return RespuestasGenerales<bool>.ErrorResponseController(errors, "Error al habilitar la autenticación de dos factores.");
            }
            return RespuestasGenerales<bool>.SuccessResponse(true, "Autenticación de dos factores habilitada exitosamente.");
        }

        public async Task<RespuestasGenerales<TokenDto>> ValidateTwoFactorAuthAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return RespuestasGenerales<TokenDto>.ErrorResponseService("Usuario", "El usuario no existe.", "Unauthorized");
        
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);

            if (!isValid) return RespuestasGenerales<TokenDto>.ErrorResponseService("Código", "Código de autenticación inválido.");

            var token = await _tokenService.CreateTokenAsync(user);
            //si el token es válido, se guarda en la base de datos
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);
            return  RespuestasGenerales<TokenDto>.SuccessResponse(new TokenDto { Token = token }, "Autenticación de dos factores validada exitosamente.");
        }

        public async Task<RespuestasGenerales<bool>> DesactivarDobleFactorAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return RespuestasGenerales<bool>.ErrorResponseService("Usuario", "El usuario no existe.", "Unauthorized");

            if (!user.TwoFactorEnabled) return RespuestasGenerales<bool>.ErrorResponseService("DobleFactor", "El doble factor de autenticación ya está desactivado.");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
            if (!isValid) return RespuestasGenerales<bool>.ErrorResponseService("Código", "Código de autenticación inválido.");
        
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
                return RespuestasGenerales<bool>.ErrorResponseService("Usuario", "Error al desactivar el doble factor de autenticación.", "statusCode500");

            return RespuestasGenerales<bool>.SuccessResponse(true, "Doble factor de autenticación desactivado exitosamente.");
        }
    }
}