
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Models;
using ApiUCI.Dtos.Auth;

namespace ApiUci.Interfaces
{
    public interface IAuthService
    {
        Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto, string ipAddress);
        Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(string userId, string password);
        Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
        Task<RespuestasGenerales<bool>> LogoutAsync(string userId);
        Task<RespuestasGenerales<QrCodeUri>> GenerarTwoFactorAuthAsync(string userId,  HttpRequest request);
        Task<RespuestasGenerales<bool>> EnableTwoFactorAuthAsync(string userId, string code);
        Task<RespuestasGenerales<TokenDto>> ValidateTwoFactorAuthAsync(string userId, string code);
        Task<RespuestasGenerales<bool>> DesactivarDobleFactorAsync(string userId, string code);
    }
}