using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Helpers;
using ApiUCI.Models;

namespace ApiUCI.Interfaces
{
    public interface IUsuarioService
    {
        Task<RespuestasGenerales<UsuarioDto>> UpdateAsync(string id, UsuarioWhiteRolUpdateDto ususarioDto);
        Task<RespuestasGenerales<UsuarioDto>> DeleteUserYRolAsync(string usuarioId);
        Task<RespuestasGenerales<List<UsuarioDto>>> GetAllAsync(QueryObjectUsuario query);
        Task<RespuestasGenerales<UsuarioDto?>> GetByIdAsync(string id);
        Task<RespuestasGenerales<UsuarioDto?>> DeleteAsync(string id);
        Task<RespuestasGenerales<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto); 
    }
}