
using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IAuthService
    {
        Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto);
        Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(string userId, string password);
        Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
        Task<RespuestasGenerales<bool>> LogoutAsync(string userId);
    }
}