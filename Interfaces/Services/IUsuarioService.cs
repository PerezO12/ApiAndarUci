using ApiUci.Dtos;
using ApiUci.Dtos.Cuentas;
using ApiUci.Dtos.Usuarios;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IUsuarioService
    {
        Task<RespuestasGenerales<UsuarioDto>> UpdateAsync(string id, UsuarioWhiteRolUpdateDto ususarioDto);
        Task<RespuestasGenerales<UsuarioDto>> DeleteUserYRolAsync(string usuarioId);
        Task<RespuestasGenerales<List<UsuarioDto>>> GetAllAsync(QueryObjectUsuario query);
        Task<RespuestasGenerales<UsuarioDto?>> GetByIdAsync(string id);
        Task<RespuestasGenerales<UsuarioDto?>> DeleteAsync(string id);
        Task<RespuestasGenerales<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto); 
        Task<RespuestasGenerales<bool>> CambiarPasswordUsuario(AppUser usuario, string newPassword);
    }
}