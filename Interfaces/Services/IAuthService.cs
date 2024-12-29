
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IAuthService
    {
        Task<RespuestasGenerales<UserPerfilDto>> Login(LoginDto loginDto);
        Task<RespuestasGenerales<UserPerfilDto>> ObtenerPerfilAsync(string id);
        Task<bool> VerifyUserPassword(string userId, string password);
        Task<RespuestasGenerales<bool>> CambiarPasswordAsync(string usuarioId, CambiarPasswordDto cuentaDto);
        Task<AppUser?> ExisteUsuario(string userId);
    }
}